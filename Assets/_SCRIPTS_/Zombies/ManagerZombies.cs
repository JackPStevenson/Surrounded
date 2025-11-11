using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(ZombiesPool))]
public class ManagerZombies : MonoBehaviour {
    public static ManagerZombies Instance;
    private ZombiesPool _zombiesPool;
    private ManagerWave _managerWave;
    
    // --- DELEGATES ---

    [Header("References")]
    public GameObject zombieBasePrefab;
    public DataStatusEffect zombieScalarStatusEffect;
    public DataBundleMaster masterBundle;

    [Header("Targeting")]
    public float approachDistance;
    public LayerMask sideTargetMask;
    private Health _mainTarget;
    public Health MainTarget => _mainTarget;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public List<DataZombie> ZombieDataEntries => masterBundle.BundleZombies.Zombies;
    public int ActiveZombieCount => _zombiesPool.ActiveZombieCount;
    private int _spawnRandSeed = 0;

    private int _tick;
    
    // ------ START FUNCTIONS ------

    void Awake() {
        Instance = this;
    }

    void Start() {
        _managerWave = ManagerWave.Instance;
        TryGetComponent(out _zombiesPool);
        
        _managerWave.OnHordeSpawn += SpawnZombieHorde;
        _managerWave.EventTrickleSpawn += SpawnZombieRandom;
        _managerWave.OnAllZombiesDead += DepletePool;
    }
    
    // ------ UPDATE FUNCTIONS ------

    void Update() {
        _zombiesPool.UpdateCustom(Time.deltaTime);
    }

    void FixedUpdate() {
        _zombiesPool.FixedUpdateCustom(Time.deltaTime, _tick);
        _tick++;
    }

    // ------ EVENT FUNCTIONS ------

    /// Destroys all zombies in or made from pool.
    public void DepletePool() => _zombiesPool.Deplete();
    
    // ------ ZOMBIE SPAWNING/RETURNING ------
    
    /// Spawns a zombie with random data from entries based on current wave.
    public void SpawnZombieRandom(int currentWave = 1) => SpawnZombieRandomWithReturn(currentWave);
    public ZombieCore SpawnZombieRandomWithReturn(int currentWave = 1) => GetZombieData(currentWave, out int index) ? SpawnZombieSpecific(index, currentWave) : null;
    
    /// Spawns a zombie type with data at given data index.
    private ZombieCore SpawnZombieSpecific(int dataIndex, int currentWave) {
        ZombieCore z = _zombiesPool.Get(ZombieDataEntries[dataIndex], spawnPoints[Random.Range(0, spawnPoints.Length)].position);
        z.TryAddEffect(zombieScalarStatusEffect, false, currentWave - 1);
        return z;
    }
    
    public void SpawnZombieHorde(int currentWave, int count, float delayBetweenSpawns) => StartCoroutine(SpawnHordeTapered(currentWave, count, delayBetweenSpawns));
    
    private IEnumerator SpawnHordeTapered(int currentWave, int count, float delayBetweenSpawns) {
        for (int i = 0; i < count; i++) {
            SpawnZombieRandom(currentWave);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }
    
    // ------ HELPER FUNCTIONS ------
    
    /// Attempts to get a random zombie data entry that can be spawned this wave.
    private DataZombie GetZombieData(int currentWave, out int selectedIndex) {
        selectedIndex = -1;
        float totalWeight = 0;
        List<int> validEntries = new List<int>();
        
        // Find all data entries that can be spawned this wave.
        for (int i = 0; i < ZombieDataEntries.Count; i++) {
            if (ZombieDataEntries[i].minimumSpawnWave > currentWave) continue;
            totalWeight += ZombieDataEntries[i].spawnWeight;
            validEntries.Add(i);
        }
        
        // If at least 1 entry was found, select a random number and loop through each valid entry.
        if(validEntries.Count <= 0) return null;
        float rand = Rand(0, totalWeight);

        foreach (int i in validEntries) {
            // If currently examined entry's weight exceeds rand, return it.
            rand -= ZombieDataEntries[i].spawnWeight;
            if (rand > 0) continue;
            selectedIndex = i;
            return ZombieDataEntries[i];
        }

        // Return final valid entry as a failsafe.
        selectedIndex = validEntries.Count - 1;
        return ZombieDataEntries[validEntries[^1]];
    }
    
    private float Rand(float min, float max) {
        Random.InitState(_spawnRandSeed);
        _spawnRandSeed++;
        return Random.Range(min, max);
    }
    
    public void SetMainTarget(Health newTarget) => _mainTarget = newTarget;
}