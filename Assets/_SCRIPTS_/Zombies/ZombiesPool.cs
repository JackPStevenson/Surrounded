using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Pool;
using Object = UnityEngine.Object;

[RequireComponent(typeof(ManagerZombies))]
public class ZombiesPool : MonoBehaviour, IUpdateCustom {
    public event Action<ZombieCore> EventZombieSpawned;
    public event Action<DataZombie> EventZombieReturned;
    public event Action EventPoolDepleted;
    
    private ManagerZombies _manager;
    
    // --- CONTENTS ---
    private List<ZombieCore> _activeZombies;
    private Queue<ZombieCore> _pooledZombies;
    
    public int ActiveZombieCount => _activeZombies.Count;
    public int PooledZombieCount => _pooledZombies.Count;

    // ------ CONSTRUCTOR ------

    void Awake() {
        TryGetComponent(out _manager);
        
        _activeZombies = new List<ZombieCore>();
        _pooledZombies = new Queue<ZombieCore>();
    }
    
    // ------ UPDATE FUNCTIONS ------

    public void UpdateCustom(float deltaTime) {
        if (_activeZombies == null || ActiveZombieCount <= 0) return;
        for (int index = 0; index < _activeZombies.Count; index++) {
            ZombieCore z = _activeZombies[index];
            z.UpdateCustom(deltaTime);
            if (!z.IsAlive) index--;
        }
    }
    
    public void FixedUpdateCustom(float deltaTime, int tick) {
        if (_activeZombies == null || ActiveZombieCount <= 0) return;
        for (int index = 0; index < _activeZombies.Count; index++) {
            ZombieCore z = _activeZombies[index];
            z.FixedUpdateCustom(deltaTime, tick);
            if (!z.IsAlive) index--;
        }
    }
    
    // ------ POOL FUNCTIONS ------

    /// Creates new pooled zombie for first time (and whenever the pool needs more).
    public ZombieCore Get(DataZombie zombieData, Vector3 spawnPos) {
        // If there are any pooled zombies, fetch them. Otherwise, make a new zombie.
        if (PooledZombieCount > 0) {
            ZombieCore zombie = _pooledZombies.Dequeue();
            return SpawnZombie(zombie, zombieData, spawnPos);
        }
        else {
            Instantiate(_manager.zombieBasePrefab).TryGetComponent(out ZombieCore zombie);
            zombie.Initialize(ActiveZombieCount + PooledZombieCount, _manager.MainTarget, _manager.approachDistance, _manager.sideTargetMask);
            zombie.EventDeath += Release;
            
            return SpawnZombie(zombie, zombieData, spawnPos);
        }
    }
    
    public void Release(CharacterCore zombie, string deathSource = "") {
        ZombieCore z = zombie as ZombieCore;
        _activeZombies.Remove(z);
        _pooledZombies.Enqueue(z);
        EventZombieReturned?.Invoke(z?.Data);
    }
    
    public void Deplete() {
        foreach (ZombieCore t in _activeZombies.Where(t => t)) Destroy(t.gameObject);
        foreach (ZombieCore t in _pooledZombies.Where(t => t)) Destroy(t.gameObject);
        _activeZombies.Clear();
        _pooledZombies.Clear();
        EventPoolDepleted?.Invoke();
    }

    // ------ HELPER FUNCTIONS ------
    
    ZombieCore SpawnZombie(ZombieCore zombie, DataZombie zombieData, Vector3 spawnPos) {
        zombie.Spawn(zombieData, spawnPos);
        EventZombieSpawned?.Invoke(zombie);
        _activeZombies.Add(zombie);
        return zombie;
    }
}