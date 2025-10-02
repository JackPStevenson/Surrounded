using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBase : Damageable {
    public ZombieStateDelegate OnZombieStateChanged;
    
    private const float CharacterRadius = 0.4f;
    
    private ZombieManager _manager;

    private ZombieDataEntry _data;
    
    private Damageable _mainTarget;
    private Damageable _sideTarget;
    private LayerMask _sideTargetMask;
    
    private NavMeshAgent _nav;
    
    private Vector3 _moveToPos;
    private Vector3 _lastTargetPos;
    private float _targetApproachPosDistance;

    private float _attackRange;
    private float _lastAttack;

    private ZombieState _zombieState = ZombieState.Approaching;

    // ------ START FUNCTIONS ------

    protected override void OnStart() {
        _manager = ZombieManager.Instance;
        OnDeath += ReturnToPool;
    }

    public void Initialize(ZombieDataEntry zombieData, Vector3 spawnPos, Damageable mainTarget) {
        // Update zombie's data, position, and current health.
        _data = zombieData;
        transform.position = spawnPos;
        _mainTarget = mainTarget;
        
        maxHealth = _data.health;
        CurrentHealth = zombieData.health;
        
        // Update nav agent and its speed and engage approach mode.
        TryGetComponent(out _nav);
        _nav.speed = zombieData.speed;
        ChangeZombieState(ZombieState.Approaching, false);
        
        
        // Create visual element for zombie.
        Instantiate(zombieData.visualPrefab, transform);
        SetActive(false);
        
        // Calculate how far zombie should be from target (with slight variance to reduce pileups).
        _attackRange = (CharacterRadius * 2) + (_data.attackRange * Random.Range(0.9f, 1f));
        _nav.stoppingDistance = 0.0f;
        if(_mainTarget)
            _lastTargetPos = _mainTarget.Position + Vector3.up * 5;
        
        _sideTargetMask = LayerMask.GetMask("Barricade");
    }

    // ------ UPDATE FUNCTIONS ------

    public void UpdateLoop() {

    }

    public void FixedUpdateLoop() {
        if (!_mainTarget)
            return;
        
        // If target moves too far from last recorded target position, update nav destination.
        if (Vector3.Distance(_lastTargetPos, _mainTarget.Position) > 0.1f)
            UpdateMoveToPos();
        
        if(_nav.pathPending)
            return;
        
        // Always check for side targets since they can be placed at any moment.
        CheckForSideTarget();

        switch (_zombieState) {
            // --- APPROACH MODE ---
            default: case ZombieState.Approaching:
                _nav.speed = _data.speed;
                
                // Check if zombie has either reached approach point or gotten close enough to player.
                bool closeToApproachPos = _nav.remainingDistance <= 0.25f;
                bool closeToTargetPos = GetDistanceToMainTarget() <= _targetApproachPosDistance * 1.1f;
                
                // If either above criteria are true, switch to charge mode.
                if (closeToApproachPos || closeToTargetPos)
                    ChangeZombieState(ZombieState.Charging);
                break;
        
            // --- CHARGE MODE ---
            case ZombieState.Charging:
                _nav.speed = _data.speed;
                
                if (GetDistanceToMainTarget(true, true) <= 0) {
                    _lastAttack = Time.time;
                    ChangeZombieState(ZombieState.AttackingMainTarget);
                }
                break;
            
            // --- ATTACKING SIDE TARGET MODE ---
            case ZombieState.AttackingSideTarget:
                _nav.speed = 0;
                
                if (_sideTarget)
                    TryAttack(_sideTarget, true);
                else
                    ChangeZombieState(ZombieState.Approaching);
                break;
            
            // --- ATTACKING MAIN TARGET MODE ---
            case ZombieState.AttackingMainTarget:
                _nav.speed = 0;
                
                if (_mainTarget) TryAttack(_mainTarget, true);
                break;
        }
    }

    // ------ EVENT FUNCTIONS ------

    void UpdateMoveToPos() {
        if (!_mainTarget) return;
        // Get position of main target.
        _lastTargetPos = _mainTarget.Position;
        _moveToPos = _lastTargetPos;
        
        // If zombie is in approach mode, add offset to main target's position for path variation.
        if (_zombieState is ZombieState.Approaching) {
            Vector2 targetPosOffset = Random.insideUnitCircle.normalized * (_targetApproachPosDistance + (CharacterRadius * 2));
            _moveToPos = _mainTarget.Position + new Vector3(targetPosOffset.x, 0, targetPosOffset.y);
        
            // Sample target pos on navmesh to ensure point is on navmesh.
            NavMesh.SamplePosition(_moveToPos, out NavMeshHit hit, 10f, NavMesh.AllAreas);
            _moveToPos = hit.position;
        }
        
        _nav.stoppingDistance = _zombieState is ZombieState.Approaching ? 0f : _attackRange;
        
        _nav.SetDestination(_moveToPos);
    }

    void ChangeZombieState(ZombieState newState, bool changeTargetPos = true) {
        _zombieState = newState;
        
        if(changeTargetPos)
            UpdateMoveToPos();
        
        OnZombieStateChanged?.Invoke(_zombieState);
    }

    /// Checks if a side target is blocking zombie's path. Acts accordingly if so.
    void CheckForSideTarget() {
        Vector3 capsuleTop = Position +  (Vector3.up * _nav.height);
        Vector3 targetDir;
        float checkDist;
        
        // If zombie is approaching, check right in front of where they are heading.
        if (_zombieState == ZombieState.Approaching) {
            targetDir = _nav.desiredVelocity.normalized;
            checkDist = 0.1f;
        }
        // Otherwise, check path between zombie and player directly.
        else {
            targetDir = (_mainTarget.Position - Position).normalized;
            checkDist = Mathf.Min(GetDistanceToMainTarget());
        }
        
        // Only proceed if side target was found and has damageable component.
        if (!Physics.CapsuleCast(Position, capsuleTop, CharacterRadius, targetDir, out RaycastHit hit, checkDist, _sideTargetMask)) return;
        if (!hit.transform.TryGetComponent(out Damageable d)) return;
        
        // Have zombie start attacking side target.
        _sideTarget = d;
        ChangeZombieState(ZombieState.AttackingSideTarget);
    }

    /// Attempt to attack current target. Will wait for attack interval to fully elapse.
    void TryAttack(Damageable target, bool stopOnDeath = false) {
        // Only deal damage if attack interval has fully elapsed.
        if (_lastAttack + _data.attackInterval > Time.time) return;
        float healthLeft = target.DealDamage(_data.attackDamage);
        _lastAttack = Time.time;

        // If target dies and stop on death is false, continue moving zombie.
        if (healthLeft > 0 || stopOnDeath) return;
        ChangeZombieState(ZombieState.Approaching);
    }

    // ------ POOLING ------

    /// Returns zombie back to pool with its data erased. This should only be called by Zombie Manager script.
    public void ReturnToPool() {
        if (IsInPool()) return;

        _data = null;
        _manager.ReturnZombie(this);
    }

    // ------ HELPER FUNCTIONS ------

    // Returns distance between zombie and main target with optional inclusion of character radius and attack range. Negative value means zombie is within range.
    float GetDistanceToMainTarget(bool includeCharacterRadius = true, bool includeAttackRange = false) {
        // Calculate raw distance from zombie to main target.
        float distToTarget = Vector3.Distance(Position, _mainTarget.Position);

        // Reduce distance based on optional parameters.
        if (includeCharacterRadius) distToTarget -= CharacterRadius * 2;
        if(includeAttackRange) distToTarget -= _nav.stoppingDistance;
        
        // Return distance.
        return distToTarget;
    }
    
    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    public bool IsInPool() => !_data;
    
    public float SetTargetPosDeviation(float newDeviation) => _targetApproachPosDistance = newDeviation;
    
    public ZombieState GetZombieState() => _zombieState;
}