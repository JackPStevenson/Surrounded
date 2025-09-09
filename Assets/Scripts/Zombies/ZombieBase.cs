using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ZombieBase : Damageable {
    private ZombieManager _manager;

    private NavMeshAgent _nav;

    private Damageable _target;
    private Vector2 lastTargetPos;


    private ZombieDataEntry _data;

    // ------ START FUNCTIONS ------

    new void Start() {
        base.Start();
        
        _manager = ZombieManager.Instance;
    }

    public void Initialize(ZombieDataEntry zombieData, Vector3 spawnPos) {
        transform.position = spawnPos;
        
        _data = zombieData;
        CurrentHealth = zombieData.maxHealth;
        
        if(!_nav) _nav = GetComponent<NavMeshAgent>();
        _nav.speed = zombieData.speed;
        
        // Attempt to create visual element.
        Instantiate(zombieData.visualPrefab, transform);

        OnDeath += ReturnToPool;
        
        SetActive(false);
    }

    private void OnDisable() {
        
    }

    // ------ UPDATE FUNCTIONS ------

    public void UpdateLoop() {

    }

    public void FixedUpdateLoop() {
        // If zombie has target and target moves, update nav destination.
        if (_target) {
            if (Vector3.Distance(lastTargetPos, _nav.destination) > 0.01f) {
                lastTargetPos = _target.Position;
                _nav.SetDestination(lastTargetPos);
            }
        }
    }

    // ------ EVENT FUNCTIONS ------

    
    
    // ------ POOLING ------
    
    /// Returns zombie back to pool with its data erased. This should only be called by Zombie Manager script.
    public void ReturnToPool() {
        _data = null;
        
        _manager.ReturnZombie(this);
    }

    // ------ HELPER FUNCTIONS ------

    public void PlaceOnNavMesh(Vector3 pos) {
        _nav.Warp(pos);
    }
    
    public void SetTarget(Damageable target) { 
        _target = target;
        lastTargetPos = _target.Position - Vector3.one;
    }
    
    public void SetActive(bool active) {
        gameObject.SetActive(active);
    }

    void OnDestroy() {
        Debug.LogWarning("ZombieBase should not be destroyed on its own. Instead, return it to pool.");
    }
}