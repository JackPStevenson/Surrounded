using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBase : Damageable {
    private ZombieManager _manager;

    private ZombieDataEntry _data;
    
    private Damageable _target;
    private NavMeshAgent _nav;
    
    private Vector2 _lastTargetPos;

    // ------ START FUNCTIONS ------

    protected override void OnStart() {
        _manager = ZombieManager.Instance;
        OnDeath += ReturnToPool;
    }

    public void Initialize(ZombieDataEntry zombieData, Vector3 spawnPos) {
        // Update zombie's data, position, and current health.
        _data = zombieData;
        transform.position = spawnPos;
        
        maxHealth = _data.maxHealth;
        CurrentHealth = zombieData.maxHealth;
        
        // Update nav agent and its speed.
        TryGetComponent(out _nav);
        _nav.speed = zombieData.speed;
        
        // Create visual element for zombie.
        Instantiate(zombieData.visualPrefab, transform);
        SetActive(false);
    }

    // ------ UPDATE FUNCTIONS ------

    public void UpdateLoop() {

    }

    public void FixedUpdateLoop() {
        // If zombie has target and target moves, update nav destination.
        if (_target && Vector3.Distance(_lastTargetPos, _nav.destination) > 0.01f) {
            _lastTargetPos = _target.Position;
            _nav.SetDestination(_lastTargetPos);
        }
    }

    // ------ EVENT FUNCTIONS ------

    
    
    // ------ POOLING ------

    /// Returns zombie back to pool with its data erased. This should only be called by Zombie Manager script.
    public void ReturnToPool() {
        if (IsInPool()) return;

        _data = null;
        _manager.ReturnZombie(this);
    }

    // ------ HELPER FUNCTIONS ------
    
    public void SetTarget(Damageable target) { 
        _target = target;
        _lastTargetPos = _target.Position - Vector3.one;
    }
    
    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    public bool IsInPool() => !_data;
}