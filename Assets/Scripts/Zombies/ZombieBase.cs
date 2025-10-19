using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBase : Damageable, IStatusHandling {
    private readonly int TicksPerPhysicsCheck = 4;
    
    public int ZombieId {get; private set;}
    
    private const float CharacterRadius = 0.4f;
    
    public ZombieStateDelegate OnZombieStateChanged;
    public GenericDelegate OnAttack;
    
    private ZombieState _zombieState = ZombieState.Moving;
    
    // --- ISTATUSHANDLING ---
    public FloatDelegate OnStatusDamageDelegate { get; }
    List<StatusEffect> IStatusHandling.StatusEffects { get; }
    float IStatusHandling.SpeedMod { get; set; }
    float IStatusHandling.DamageMod { get; set; }
    float IStatusHandling.ResistMod { get; set; }
    
    // --- GLOBAL REFERENCES ---
    private ZombieManager _manager;
    private ZombieDataTypes _data;
    
    // --- COMPONENTS ---
    private NavMeshAgent _nav;
    protected NavMeshAgent Agent { get { if (!_nav) TryGetComponent(out _nav); return _nav; } }
    private ZombieVisual _visual;
    
    // --- MOVEMENT ---
    private Vector3 _navTargetPos;
    private float _approachDistance;
    private bool _isApproaching;
    
    // --- TARGETING ---
    private Damageable _currentTarget;
    private Damageable _poorSoul;
    private LayerMask _sideTargetMask;
    private float _attackRange;
    private float _lastAttack;
    
    
    // ------ START FUNCTIONS ------

    protected override void OnStart() {
        _manager = ZombieManager.Instance;
        OnDeath += ReturnToPool;
    }

    public void Initialize(ZombieDataTypes zombieData, Vector3 spawnPos, float approachDist, Damageable poorSoul, LayerMask sideTargetMask) {
        // --- CONSTRUCTOR INPUTS ---
        _data = zombieData;
        transform.position = spawnPos;
        _poorSoul = poorSoul;
        _sideTargetMask = sideTargetMask;
        
        // --- HEALTH ---
        maxHealth = _data.health;
        CurrentHealth = zombieData.health;

        // --- NAV AGENT ---
        Agent.speed = zombieData.speed;
        Agent.updateRotation = false;
        Agent.stoppingDistance = 0.0f;
        
        // --- TARGETING ---
        // Make attack range measure distance between the closest point on each capsule. Add random offset to reduce zombie clumping.
        _currentTarget = null;
        _attackRange = (CharacterRadius * 2) + (_data.attackRange * Random.Range(0.8f, 1f));
        
        // Make agent's first target point randomly offset from primary target to make zombie pathing more interesting.
        _approachDistance = approachDist;
        Vector2 approachOffset = Random.insideUnitCircle.normalized * (_approachDistance + (CharacterRadius * 2));
        _navTargetPos = _poorSoul.Position + new Vector3(approachOffset.x, 0, approachOffset.y);
        _isApproaching = true;
        
        // --- VISUAL ---
        Instantiate(zombieData.visualPrefab, transform).TryGetComponent(out _visual);
        _visual.Initialize(this);

        // --- STATE ---
        ChangeZombieState(ZombieState.Moving, false);
        SetActive(false);
    }

    public void SetId(int id) {
        ZombieId = id;
        gameObject.name = "Zombie " + ZombieId; 
    }
    
    // ------ GENERAL ------

    void ChangeZombieState(ZombieState newState, bool forceUpdateNavTarget = false) {
        _zombieState = newState;
        _lastAttack = Time.time;
        
        // Update agent based on current state. If desired, forcefully update agent target.
        Agent.stoppingDistance = _isApproaching ? 0f : _attackRange;
        if(Agent.hasPath) Agent.isStopped = _zombieState is ZombieState.Attacking;
        if (forceUpdateNavTarget) UpdateAgentTarget();
        
        OnZombieStateChanged?.Invoke(_zombieState);
    }

    // ------ UPDATE FUNCTIONS ------

    public void UpdateLoop() { }

    public void FixedUpdateLoop(int tick) {
        if (_visual) _visual.FixedUpdateLoop(); // Update visual if zombie has one.
        
        
        if (!_poorSoul || _nav.pathPending || !_nav.isOnNavMesh) return; // Only continue if poor soul is still alive and zombie isn't processing a path..

        // If target moves too far from last recorded target position, update nav destination.
        if (Vector3.Distance(_navTargetPos, _poorSoul.Position) > 0.15f || Agent.destination != _navTargetPos)
            UpdateAgentTarget();

        // If zombie is allowed to do physics check this tick, check for possible damageables in the way. If one is found, make zombie attack it.
        if (CheckIfPhysicsTick(tick) && CheckForSideTargets())
            ChangeZombieState(ZombieState.Attacking);

        // Perform logic based on current zombie state.
        switch (_zombieState) {
            default:
            case ZombieState.Moving: MovingBehavior(); break;
            case ZombieState.Attacking: AttackingBehavior(); break;
        }
    }

    /// Returns whether zombie should perform physics checks this tick.
    bool CheckIfPhysicsTick(int tick) {
        if (TicksPerPhysicsCheck < 2) return true;
        
        int offset = ZombieId % TicksPerPhysicsCheck;
        return (tick + offset) % TicksPerPhysicsCheck == 0;
    }

    // ------ BEHAVIORS ------

    public void ProcessEffects() {
        
    }

    void MovingBehavior() {
        if (_isApproaching && Agent.hasPath) {
            // Check if zombie has either reached approach point or gotten close enough to player.
            bool closeToApproachPos = Agent.remainingDistance <= 0.1f;
            bool closeToPrimaryTarget = GetDistanceToPoorSoul() <= _approachDistance * 1.1f;
            
            // If either above criteria are true, start directly moving towards main target.
            if (closeToApproachPos || closeToPrimaryTarget)
                _isApproaching = false;
        }

        // If close enough to poor soul and not targeting anything else, start attacking poor soul.
        if (GetDistanceToPoorSoul(true, true) <= 0 && !_currentTarget) _currentTarget = _poorSoul;
        
        // Make sure that zombie starts attacking if they have a target.
        if(_currentTarget) ChangeZombieState(ZombieState.Attacking);
    }
    
    void AttackingBehavior() {
        // If current target no longer exists or is killed by zombie, switch back to moving.  
        if (!_currentTarget || Mathf.Approximately(TryAttack(), 0))
            ChangeZombieState(ZombieState.Moving);
    }

    // ------ MOVEMENT ------

    // Updates agent target position and stopping distance. 
    void UpdateAgentTarget() {
        if(!_isApproaching) _navTargetPos = _poorSoul.Position;
        Agent.stoppingDistance = _isApproaching ? 0f : _attackRange;
        Agent.SetDestination(_navTargetPos);
    }
    
    // ------ TARGETING ------

    /// Checks if there's any damageables in front of zombie and sets them as current target. Returns whether zombie found a new target.
    readonly Collider[] _hitCol = new Collider[8];
    bool CheckForSideTargets() {
        Vector3 capsuleTop = Position + (Vector3.up * Agent.height);

        // Check if a valid transform overlaps with zombie. If a new side target was set from found transform, return true.
        if (Physics.OverlapCapsuleNonAlloc(Position, capsuleTop, CharacterRadius, _hitCol, _sideTargetMask) > 0)
            if (TrySetTarget(_hitCol[0].transform))
                return true;

        // If zombie is approaching, check towards their move direction. Otherwise, check towards their main target.
        Vector3 targetDir = _isApproaching ? _nav.desiredVelocity.normalized : (_poorSoul.Position - Position).normalized;
        float checkDist = _isApproaching ? 0.1f : Mathf.Min(GetDistanceToPoorSoul(), _attackRange);

        // Check if a valid transform is in front of zombie within given range. If a new side target was set from found transform, return true.
        if (Physics.CapsuleCast(Position, capsuleTop, CharacterRadius, targetDir, out RaycastHit hit, checkDist, _sideTargetMask))
            if(TrySetTarget(hit.transform))
                return true;
        
        // If no side targets were found, return false.
        return false;
    }

    /// Tries to set side target to damageable found on given transform. Returns true and updates zombie state if it was successful.
    private bool TrySetTarget(Transform possibleTarget) {
        if (!possibleTarget.TryGetComponent(out Damageable damageable) || damageable == _currentTarget) return false;
        
        _currentTarget = damageable;
        return true;
    }

    // Returns distance between zombie and main target with optional inclusion of character radius and attack range. Negative value means zombie is within range.
    float GetDistanceToPoorSoul(bool includeCharacterRadius = true, bool includeAttackRange = false) {
        // Calculate raw distance from zombie to main target.
        float distToTarget = Vector3.Distance(Position, _poorSoul.Position);

        // Reduce distance based on optional parameters.
        if (includeCharacterRadius) distToTarget -= CharacterRadius * 2;
        if (includeAttackRange) distToTarget -= _nav.stoppingDistance;
        return distToTarget;
    }
    
    // ------ ATTACKING ------
    
    /// Attempt to attack current target. Will wait for attack interval to fully elapse. Returns remaining health of hit object
    float TryAttack() {
        if (_lastAttack + _data.attackInterval > Time.time) return -1; // Only deal damage if attack interval has fully elapsed.
        
        // Damage target, reset hit timer, and return health left of hit target.
        float healthLeft = _currentTarget.DealDamage(_data.attackDamage);
        _lastAttack = Time.time;
        OnAttack?.Invoke();
        return healthLeft;
    }

    // ------ POOLING ------

    /// Returns zombie back to pool with its data erased. This should only be called by Zombie Manager script.
    public void ReturnToPool() {
        if (IsInPool()) return;

        if (_visual) Destroy(_visual.gameObject);
        _data = null;
        _visual = null;
        _manager.ReturnZombie(this);
    }

    // ------ HELPER FUNCTIONS ------
    
    public void SetActive(bool active) => gameObject.SetActive(active);
    public bool IsInPool() => !_data;
    public ZombieState GetZombieState() => _zombieState;
    public float GetMaxSpeed() => _nav.speed;
    public Vector3 GetVelocity() => _nav.velocity;
}