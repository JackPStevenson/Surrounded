using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ManagerZombies : MonoBehaviour {
    private event Action<float> UpdateCustom;
    private event Action<float, int> FixedUpdateCustom;

    public static ManagerZombies Instance;

    private ZombiesPool _zombiesPool;
    private ManagerWave _managerWave;
    
    // --- DELEGATES ---

    [Header("References")]
    public GameObject zombieBasePrefab;

    [FormerlySerializedAs("_targetPosDeviation")] [Header("Targeting")]
    public float _approachDistance;
    public LayerMask _sideTargetMask;
    Health _mainTarget;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public DataZombie[] zombieDataEntries;
    private List<ZombieCore> _activeZombies;
    public int ActiveZombieCount => _activeZombies.Count;
    private int _spawnRandSeed = 0;

    private int _zombieTick;
    
    // ------ START FUNCTIONS ------

    void Awake() {
        Instance = this;
        _activeZombies = new List<ZombieCore>();
    }

    void Start() {
        _managerWave = ManagerWave.Instance;
        _mainTarget = PoorSoulCore.Instance.Health;
        _zombiesPool = new ZombiesPool(this, zombieBasePrefab, _mainTarget, _approachDistance, _sideTargetMask);
        
        _managerWave.OnHordeSpawn += SpawnZombieHorde;
        _managerWave.OnTrickleSpawn += SpawnZombieRandom;
        _managerWave.OnAllZombiesDead += DepletePool;
    }
    
    // ------ UPDATE FUNCTIONS ------

    void Update() {
        UpdateCustom?.Invoke(Time.deltaTime);
    }

    void FixedUpdate() {
        FixedUpdateCustom?.Invoke(Time.fixedDeltaTime, _zombieTick);
        _zombieTick++;
    }

    // ------ EVENT FUNCTIONS ------

    /// Destroys all zombies in or made from pool.
    public void DepletePool() {
        _zombiesPool.DepletePool(this);
        _activeZombies.Clear();
        
        UpdateCustom = null;
        FixedUpdateCustom = null;
    }
    

    // ------ ZOMBIE SPAWNING/RETURNING ------
    
    /// Spawns a zombie with random data from entries based on current wave.
    public void SpawnZombieRandom(int currentWave) {
        GetZombieData(currentWave, out int index);
        SpawnZombieSpecific(index);
    }
    
    /// Spawns a zombie with random data from entries based on current wave and returns spawned zombie.
    public ZombieCore SpawnZombieRandomWithReturn(int currentWave) {
        DataZombie data = GetZombieData(currentWave, out int index);
        if (!data) return null;

        return SpawnZombieSpecific(index);
    }
    
    /// Spawns a zombie type with data at given data index.
    private ZombieCore SpawnZombieSpecific(int dataIndex) {
        DataZombie data = zombieDataEntries[Mathf.Clamp(dataIndex, 0, zombieDataEntries.Length - 1)];

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        ZombieCore z = _zombiesPool.Pop(data, spawn.position);
        
        UpdateCustom += z.UpdateCustom;
        FixedUpdateCustom += z.FixedUpdateCustom;
        z.SetActive(true);
        
        _activeZombies.Add(z);
        
        return z;
    }
    
    public void SpawnZombieHorde(int currentWave, int count, float delayBetweenSpawns) {
        StartCoroutine(SpawnHordeTapered(currentWave, count, delayBetweenSpawns));
    }

    private IEnumerator SpawnHordeTapered(int currentWave, int count, float delayBetweenSpawns) {
        for (int i = 0; i < count; i++) {
            SpawnZombieRandomWithReturn(currentWave);
            yield return new WaitForSeconds(delayBetweenSpawns);
        }
    }
    
    /// Returns given zombie to pool.
    public void Push(ZombieCore zombie) {
        UpdateCustom -= zombie.UpdateCustom;
        FixedUpdateCustom -= zombie.FixedUpdateCustom;

        _zombiesPool.Push(zombie);
        _activeZombies.Remove(zombie);
    }
    
    // ------ HELPER FUNCTIONS ------
    
    /// Attempts to get a random zombie data entry that can be spawned this wave.
    private DataZombie GetZombieData(int currentWave, out int selectedIndex) {
        selectedIndex = -1;
        float totalWeight = 0;
        List<int> validEntries = new List<int>();
        
        // Find all data entries that can be spawned this wave.
        for (int i = 0; i < zombieDataEntries.Length; i++) {
            DataZombie types = zombieDataEntries[i];
            
            if (types.minimumSpawnWave > currentWave) continue;
            
            totalWeight += types.spawnWeight;
            validEntries.Add(i);
        }

        // Only continue if at least 1 entry was found.
        if(validEntries.Count <= 0) return null;

        // Select a random number and loop through each valid entry. Ensure seed is incremented.
        Random.InitState(_spawnRandSeed);
        float rand = Random.Range(0, totalWeight);
        _spawnRandSeed++;
        
        foreach (int i in validEntries) {
            DataZombie dataZombie = zombieDataEntries[i];
            
            // If currently examined entry's weight exceeds rand, return it.
            rand -= dataZombie.spawnWeight;
            if (rand > 0) continue;
                selectedIndex = i;
                return dataZombie;
        }
        
        // Return final valid entry as a failsafe in case something wrong happens.
        selectedIndex = validEntries.Count - 1;
        return zombieDataEntries[validEntries[^1]];
    }
    
    public void SetMainTarget(Health newTarget) => _mainTarget = newTarget;
}