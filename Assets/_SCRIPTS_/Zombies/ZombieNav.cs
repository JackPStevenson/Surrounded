using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public enum NavState {
    Moving,
    Attacking
}

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieNav : MonoBehaviour, IUpdateCustom {
    private const int TicksPerPhysicsCheck = 4;
    private const float CharacterRadius = 0.4f;
    
    // --- EVENTS ---
    public event Action OnAttack;

    // --- PERMANENT REFERENCES ---
    private ZombieCore _core;
    private NavMeshAgent _agent;

    private float _approachDistance;
    private LayerMask _sideTargetMask;
    
    // --- TEMPORARY REFERENCES ---
    protected DataZombie Data => _core.Data;
    private Health _mainTarget;

    // --- CURRENT STATE ---
    public NavState NavState { get; private set; } = NavState.Moving;
    public Vector3 Velocity => _agent.velocity;
    public Vector3 TargetDirection => _agent.desiredVelocity.normalized;
    
    // --- MOVEMENT ---
    private Vector3 _navTargetPos;
    private bool _isApproaching;
    
    // --- TARGETING ---
    private Health _currentTarget;
    private float _range;
    private float _lastAttack;
    
    // ------ START FUNCTIONS ------
    
    public void Initialize(ZombieCore core, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        TryGetComponent(out _agent);
        _core = core;
        
        _mainTarget = mainTarget;
        _sideTargetMask = sideTargetMask;
        _approachDistance = approachDist;
        
        _agent.updateRotation = false;
        _agent.enabled = false;
        _agent.stoppingDistance = 0.0f;
    }
    
    public void Spawn(Vector3 spawnPos) {
        _agent.enabled = false;
        transform.position = spawnPos;
        _agent.enabled = true;
        _agent.speed = _core.Speed;
        
        // Make range account for distance between closest point on each capsule w/ random offset to reduce clumping.
        _range = (CharacterRadius * 2) + (Data.attackRange * Random.Range(0.8f, 1f));
        
        // Make agent's first target point randomly offset from primary target to make zombie pathing more interesting.
        Vector2 approachOffset = Random.insideUnitCircle.normalized * (_approachDistance + (CharacterRadius * 2));
        _navTargetPos = _mainTarget.Position + new Vector3(approachOffset.x, 0, approachOffset.y);
        _isApproaching = true;

        ChangeNavState(NavState.Moving);
    }

    public void Reset() {
        _currentTarget = null;
    }
    
    // ------ GENERAL ------

    void ChangeNavState(NavState newState) {
        NavState = newState;
        _lastAttack = Time.time;

        // Update agent based on current state.
        _agent.stoppingDistance = _isApproaching ? 0f : _range;
        if (_agent.hasPath) _agent.isStopped = NavState is NavState.Attacking;
    }

    // ------ UPDATE FUNCTIONS ------
    public void UpdateCustom(float deltaTime) { }
    public void FixedUpdateCustom(float deltaTime, int tick) {
        if (!_mainTarget || !_agent.isOnNavMesh || _agent.pathPending) return; // Only continue if poor soul is still alive and zombie isn't processing a path..

        _agent.speed = _core.Speed;
        
        // If target moves too far from last recorded target position, update nav destination.
        if (Vector3.Distance(_navTargetPos, _mainTarget.Position) > 0.15f || _agent.destination != _navTargetPos)
            UpdateAgentTarget();

        // If zombie is allowed to do physics check this tick, check for possible damageables in the way. If one is found, make zombie attack it.
        if (CheckIfPhysicsTick(tick) && CheckForSideTargets())
            ChangeNavState(NavState.Attacking);

        // Perform logic based on current zombie state.
        switch (NavState) {
            default:
            case NavState.Moving: MovingBehavior(); break;
            case NavState.Attacking: AttackingBehavior(); break;
        }
    }

    /// Returns whether zombie should perform physics checks this tick.
    bool CheckIfPhysicsTick(int tick) {
        int offset = _core.Id % TicksPerPhysicsCheck;
        return (tick + offset) % TicksPerPhysicsCheck == 0;
    }

    // ------ BEHAVIORS ------

    void MovingBehavior() {
        if (_isApproaching && _agent.hasPath) {
            // Check if zombie has either reached approach point or gotten close enough to player.
            bool closeToApproachPos = _agent.remainingDistance <= 0.1f;
            bool closeToPrimaryTarget = GetDistanceToPoorSoul() <= _approachDistance * 1.1f;
            
            // If either above criteria are true, start directly moving towards main target.
            if (closeToApproachPos || closeToPrimaryTarget)
                _isApproaching = false;
        }

        // If close enough to poor soul and not targeting anything else, start attacking poor soul.
        if (GetDistanceToPoorSoul(true, true) <= 0 && !_currentTarget) _currentTarget = _mainTarget;
        
        // Make sure that zombie starts attacking if they have a target.
        if(_currentTarget) ChangeNavState(NavState.Attacking);
    }
    
    void AttackingBehavior() {
        // If current target no longer exists or is killed by zombie, switch back to moving.  
        if (!_currentTarget || Mathf.Approximately(TryAttack(), 0))
            ChangeNavState(NavState.Moving);
    }

    // ------ MOVEMENT ------

    // Updates agent target position and stopping distance. 
    void UpdateAgentTarget() {
        if(!_isApproaching) _navTargetPos = _mainTarget.Position;
        _agent.stoppingDistance = _isApproaching ? 0f : _range;
        _agent.SetDestination(_navTargetPos);
    }
    
    // ------ TARGETING ------

    /// Checks if there's any damageables in front of zombie and sets them as current target. Returns whether zombie found a new target.
    readonly Collider[] _hitCol = new Collider[8];
    bool CheckForSideTargets() {
        Vector3 capsuleTop = _core.Position + (Vector3.up * _agent.height);

        // Check if a valid transform overlaps with zombie. If a new side target was set from found transform, return true.
        if (Physics.OverlapCapsuleNonAlloc(_core.Position, capsuleTop, CharacterRadius, _hitCol, _sideTargetMask) > 0)
            if (TrySetTarget(_hitCol[0].transform))
                return true;

        // If zombie is approaching, check towards their move direction. Otherwise, check towards their main target.
        Vector3 targetDir = _isApproaching ? TargetDirection : (_mainTarget.Position - _core.Position).normalized;
        float checkDist = _isApproaching ? 0.1f : Mathf.Min(GetDistanceToPoorSoul(), _range);

        // Check if a valid transform is in front of zombie within given range. If a new side target was set from found transform, return true.
        if (Physics.CapsuleCast(_core.Position, capsuleTop, CharacterRadius, targetDir, out RaycastHit hit, checkDist, _sideTargetMask))
            if(TrySetTarget(hit.transform))
                return true;
        
        // If no side targets were found, return false.
        return false;
    }

    /// Tries to set side target to damageable found on given transform. Returns true and updates zombie state if it was successful.
    private bool TrySetTarget(Transform possibleTarget) {
        if (!possibleTarget.TryGetComponent(out Health damageable) || damageable == _currentTarget) return false;
        
        _currentTarget = damageable;
        return true;
    }

    // Returns distance between zombie and main target with optional inclusion of character radius and attack range. Negative value means zombie is within range.
    float GetDistanceToPoorSoul(bool includeCharacterRadius = true, bool includeAttackRange = false) {
        // Calculate raw distance from zombie to main target.
        float distToTarget = Vector3.Distance(_core.Position, _mainTarget.Position);

        // Reduce distance based on optional parameters.
        if (includeCharacterRadius) distToTarget -= CharacterRadius * 2;
        if (includeAttackRange) distToTarget -= _agent.stoppingDistance;
        return distToTarget;
    }
    
    // ------ ATTACKING ------
    
    /// Attempt to attack current target. Will wait for attack interval to fully elapse. Returns remaining health of hit object
    float TryAttack() {
        if (_lastAttack + _core.AttackInterval > Time.time) return -1; // Only deal damage if attack interval has fully elapsed.
        
        // Damage target, reset hit timer, and return health left of hit target.
        float healthLeft = _currentTarget.DealDamage(_core.Damage, _core.Data.name);
        _lastAttack = Time.time;
        OnAttack?.Invoke();
        return healthLeft;
    }
    
}