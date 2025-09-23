using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiePool {
    private const int MinimumZombies = 4;

    public delegate void GenericDelegate();
    public GenericDelegate OnPoolDeplete;

    private int _totalZombies;

    private Stack<ZombieBase> _zombieStack;
    private GameObject _zombieBasePrefab;

    // ------ CONSTRUCTOR ------

    public ZombiePool(GameObject zombieBasePrefab) {
        _zombieStack = new Stack<ZombieBase>();
        _zombieBasePrefab = zombieBasePrefab;
    }

    // ------ PUSH/POP ------

    /// Gets a zombie from pool.
    public ZombieBase Pop(ZombieDataEntry zombieData, Vector3 spawnPos) {
        if (_zombieStack.Count <= 0) ExpandPool();
        ZombieBase z = _zombieStack.Pop();
        z.Initialize(zombieData, spawnPos);
        return z;
    }

    /// Returns given zombie back to pool.
    public void Push(ZombieBase zombie) {
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
            ZombieBase z = Object.Instantiate(_zombieBasePrefab).GetComponent<ZombieBase>();
            OnPoolDeplete += z.ReturnToPool;
            Push(z);
        }
        

        _totalZombies += zombiesToAdd;
    }

    /// Resets pool to minimum size.
    public void DepletePool() {
        // Call all zombies back into pool and delete them.
        OnPoolDeplete?.Invoke();
        while (_zombieStack.Count > 0) {
            Object.Destroy(_zombieStack.Pop().gameObject);
        }

        // Remove all references to now deleted zombies.
        OnPoolDeplete = null;
    }

    public void RemoveFromPool(ZombieBase zombie) {
        OnPoolDeplete -= zombie.ReturnToPool;
    }
}