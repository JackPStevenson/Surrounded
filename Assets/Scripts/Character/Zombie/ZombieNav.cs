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
    protected ZombieCore Core;
    protected NavMeshAgent Agent;
    
    protected Health MainTarget;
    protected float ApproachDistance;
    protected LayerMask SideTargetMask;
    
    // --- TEMPORARY REFERENCES ---
    protected DataZombie Data => Core.Data;

    // --- CURRENT STATE ---
    public NavState NavState { get; private set; } = NavState.Moving;
    public Vector3 Velocity => Agent.velocity;
    public Vector3 TargetDirection => Agent.desiredVelocity.normalized;
    
    // --- MOVEMENT ---
    private Vector3 _navTargetPos;
    private bool _isApproaching;
    
    // --- TARGETING ---
    private Health _currentTarget;
    private float _range;
    private float _lastAttack;
    
    // ------ START FUNCTIONS ------
    
    public void Initialize(ZombieCore core, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        if (TryGetComponent(out NavMeshAgent a)) Agent = a;
        else {
            enabled = false;
            return;
        }
        
        Core = core;
        
        MainTarget = mainTarget;
        SideTargetMask = sideTargetMask;
        ApproachDistance = approachDist;
        
        Agent.updateRotation = false;
        Agent.stoppingDistance = 0.0f;
    }
    
    // ------ POOLING ------
    
    public void Pop(Vector3 spawnPos) {
        if (!Agent) {
            enabled = false;
            return;
        }
        
        transform.position = spawnPos;

        // --- NAV AGENT ---
        Agent.speed = Core.CurrentTargetSpeed;
        
        // --- TARGETING ---
        // Make range account for distance between closest point on each capsule w/ random offset to reduce clumping.
        _range = (CharacterRadius * 2) + (Data.attackRange * Random.Range(0.8f, 1f));
        
        // Make agent's first target point randomly offset from primary target to make zombie pathing more interesting.
        Vector2 approachOffset = Random.insideUnitCircle.normalized * (ApproachDistance + (CharacterRadius * 2));
        _navTargetPos = MainTarget.Position + new Vector3(approachOffset.x, 0, approachOffset.y);
        _isApproaching = true;

        // --- STATE ---
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
        Agent.stoppingDistance = _isApproaching ? 0f : _range;
        if (Agent.hasPath) Agent.isStopped = NavState is NavState.Attacking;
    }

    // ------ UPDATE FUNCTIONS ------

    public void UpdateCustom(float deltaTime) { }
    
    public void FixedUpdateCustom(float deltaTime, int tick) {
        if (!MainTarget || !Agent.isOnNavMesh || Agent.pathPending) return; // Only continue if poor soul is still alive and zombie isn't processing a path..

        Agent.speed = Core.CurrentTargetSpeed;
        
        // If target moves too far from last recorded target position, update nav destination.
        if (Vector3.Distance(_navTargetPos, MainTarget.Position) > 0.15f || Agent.destination != _navTargetPos)
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
        if (TicksPerPhysicsCheck < 2) return true;
        
        int offset = Core.Id % TicksPerPhysicsCheck;
        return (tick + offset) % TicksPerPhysicsCheck == 0;
    }

    // ------ BEHAVIORS ------

    void MovingBehavior() {
        if (_isApproaching && Agent.hasPath) {
            // Check if zombie has either reached approach point or gotten close enough to player.
            bool closeToApproachPos = Agent.remainingDistance <= 0.1f;
            bool closeToPrimaryTarget = GetDistanceToPoorSoul() <= ApproachDistance * 1.1f;
            
            // If either above criteria are true, start directly moving towards main target.
            if (closeToApproachPos || closeToPrimaryTarget)
                _isApproaching = false;
        }

        // If close enough to poor soul and not targeting anything else, start attacking poor soul.
        if (GetDistanceToPoorSoul(true, true) <= 0 && !_currentTarget) _currentTarget = MainTarget;
        
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
        if(!_isApproaching) _navTargetPos = MainTarget.Position;
        Agent.stoppingDistance = _isApproaching ? 0f : _range;
        Agent.SetDestination(_navTargetPos);
    }
    
    // ------ TARGETING ------

    /// Checks if there's any damageables in front of zombie and sets them as current target. Returns whether zombie found a new target.
    readonly Collider[] _hitCol = new Collider[8];
    bool CheckForSideTargets() {
        Vector3 capsuleTop = Core.Position + (Vector3.up * Agent.height);

        // Check if a valid transform overlaps with zombie. If a new side target was set from found transform, return true.
        if (Physics.OverlapCapsuleNonAlloc(Core.Position, capsuleTop, CharacterRadius, _hitCol, SideTargetMask) > 0)
            if (TrySetTarget(_hitCol[0].transform))
                return true;

        // If zombie is approaching, check towards their move direction. Otherwise, check towards their main target.
        Vector3 targetDir = _isApproaching ? TargetDirection : (MainTarget.Position - Core.Position).normalized;
        float checkDist = _isApproaching ? 0.1f : Mathf.Min(GetDistanceToPoorSoul(), _range);

        // Check if a valid transform is in front of zombie within given range. If a new side target was set from found transform, return true.
        if (Physics.CapsuleCast(Core.Position, capsuleTop, CharacterRadius, targetDir, out RaycastHit hit, checkDist, SideTargetMask))
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
        float distToTarget = Vector3.Distance(Core.Position, MainTarget.Position);

        // Reduce distance based on optional parameters.
        if (includeCharacterRadius) distToTarget -= CharacterRadius * 2;
        if (includeAttackRange) distToTarget -= Agent.stoppingDistance;
        return distToTarget;
    }
    
    // ------ ATTACKING ------
    
    /// Attempt to attack current target. Will wait for attack interval to fully elapse. Returns remaining health of hit object
    float TryAttack() {
        if (_lastAttack + Core.Data.attackRate > Time.time) return -1; // Only deal damage if attack interval has fully elapsed.
        
        // Damage target, reset hit timer, and return health left of hit target.
        float healthLeft = _currentTarget.ModHealth(Core.CurrentDamage);
        _lastAttack = Time.time;
        OnAttack?.Invoke();
        return healthLeft;
    }

    // ------ HELPER FUNCTIONS ------
    
}