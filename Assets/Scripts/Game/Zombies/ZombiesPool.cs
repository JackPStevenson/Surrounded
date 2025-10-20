using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ZombiesPool {
    private const int MinimumZombies = 4;

    protected ZombiesManager Manager;
    
    // --- CONTENTS ---
    private int _totalZombies;
    private readonly Stack<ZombieCore> _zombieStack;
    private readonly GameObject _zombieBasePrefab;
    private Health _mainTarget;
    private float _approachDist;
    private LayerMask _sideTargetMask;

    // --- DELEGATES ---
    private event Action OnPoolDeplete;

    // ------ CONSTRUCTOR ------
    
    public ZombiesPool(ZombiesManager manager, GameObject zombieBasePrefab, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        Manager = manager;
        _zombieStack = new Stack<ZombieCore>();
        _zombieBasePrefab = zombieBasePrefab;
        _mainTarget = mainTarget;
        _approachDist = approachDist;
        _sideTargetMask = sideTargetMask;
    }

    // ------ PUSH/POP ------

    /// Gets a zombie from pool.
    public ZombieCore Pop(ZombieData zombieData, Vector3 spawnPos) {
        if (_zombieStack.Count <= 0) ExpandPool();
        ZombieCore z = _zombieStack.Pop();
        z.Pop(zombieData, spawnPos);
        return z;
    }

    /// Returns given zombie back to pool.
    public void Push(ZombieCore zombie) {
        _zombieStack.Push(zombie);
        zombie.SetActive(false);
    }

    // ------ POOL SIZE MANAGEMENT ------

    /// Doubles the size of the zombie pool.
    void ExpandPool() {
        // Add however many zombies are needed to make list.
        int zombiesToAdd = Mathf.Max(_totalZombies, MinimumZombies);
        for (int i = 0; i < zombiesToAdd; i++) {
            // Create a zombie, add zombie's return method to pool's delegate, initialize it, and add zombie to stack.
            if (!Object.Instantiate(_zombieBasePrefab).TryGetComponent(out ZombieCore z)) return;
            
            OnPoolDeplete += z.Push;
            z.Initialize(Manager, _totalZombies + i, _mainTarget, _approachDist, _sideTargetMask);
            Push(z);
        }
        
        _totalZombies += zombiesToAdd;
    }

    /// Resets pool to minimum size.
    public void DepletePool(ZombiesManager manager) {
        // Call all zombies back into pool and delete them.
        OnPoolDeplete?.Invoke();
        
        while (_zombieStack.Count > 0) {
            Object.Destroy(_zombieStack.Pop().gameObject);
        }

        // Remove all references to now deleted zombies.
        OnPoolDeplete = null;
    }
}