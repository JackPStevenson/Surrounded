using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ZombieManager : MonoBehaviour {
    public delegate void GenericDelegate();
    public GenericDelegate OnUpdate;
    public GenericDelegate OnFixedUpdate;

    public static ZombieManager Instance;
    private GameManager _gameManager;

    private List<ZombieBase> _activeZombies;
    private ZombiePool _zombiePool;

    private int _currentWave = 1;

    [Header("References")]
    public GameObject zombieBasePrefab;

    [Header("Targeting")]
    public Damageable mainTarget;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public ZombieDataEntry[] zombieDataEntries;
    
    public int zombiesPerWaveStart = 12;
    public int zombiesPerWaveIncrement = 3;
    private int _zombiesToSpawn;
    private int _remainingZombies;
    private float _lastZombieSpawnTime;
    
    public float zombieSpawnDelay = 1.5f;
    public float zombieSpawnDelayIncrement = 1;
    
    // ------ START FUNCTIONS ------

    void Awake() {
        Instance = this;
        _activeZombies = new List<ZombieBase>();

        _zombiePool = new ZombiePool(zombieBasePrefab);
    }

    void Start() {
        _gameManager = GameManager.Instance;
        enabled = false;
    }
    
    // ------ TOGGLE FUNCTIONS ------

    void OnEnable() {
        _lastZombieSpawnTime = Time.time;
        StartNextWave();
    }
    
    void OnDisable() {
        _zombiePool.DepletePool();
    }

    // ------ UPDATE FUNCTIONS ------

    void Update() {
        OnUpdate?.Invoke();
    }

    void FixedUpdate() {
        OnFixedUpdate?.Invoke();
        
        if (Time.time > _lastZombieSpawnTime + zombieSpawnDelay) {
            SpawnZombie();
            _lastZombieSpawnTime = Time.time;
        }
    }


    // ------ EVENT FUNCTIONS ------

    public void StartNextWave() {
        _zombiesToSpawn = zombiesPerWaveStart + (zombiesPerWaveIncrement * (_currentWave - 1));
        _remainingZombies = _zombiesToSpawn;
        
        _currentWave++;
        enabled = true;
    }

    // ------ ZOMBIE SPANWING/RETURNING ------
    
    /// Spawns a zombie with random data from entries.
    public ZombieBase SpawnZombie() {
        ZombieDataEntry data = GetZombieData();
        if (!data) return null;
        

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        ZombieBase z = _zombiePool.Pop(data, spawn.position);
        z.SetTarget(mainTarget);
        
        OnUpdate += z.UpdateLoop;
        OnFixedUpdate += z.FixedUpdateLoop;

        _zombiesToSpawn--;
        
        z.SetActive(true);
        return z;
    }

    /// Returns given zombie to pool.
    public void ReturnZombie(ZombieBase zombie) {
        
        OnUpdate -= zombie.UpdateLoop;
        OnFixedUpdate -= zombie.FixedUpdateLoop;
        _remainingZombies--;
        
        zombie.ReturnToPool();
    }
    
    // ------ HELPER FUNCTIONS ------
    
    /// Attempts to get a random zombie data entry that can be spawned this wave.
    public ZombieDataEntry GetZombieData() {
        float totalWeight = 0;
        List<ZombieDataEntry> validEntries = new List<ZombieDataEntry>();
        
        // Find all data entries that can be spawned this wave.
        foreach (ZombieDataEntry entry in zombieDataEntries) {
            if (entry.minimumSpawnWave > _currentWave) continue;
            totalWeight += entry.spawnWeight;
            validEntries.Add(entry);
        }

        // Only continue if at least 1 entry was found.
        if(validEntries.Count <= 0) return null;

        // Select a random number and loop through each valid entry.
        float rand = Random.Range(0, totalWeight);
        foreach (ZombieDataEntry entry in validEntries) {
            // If currently examined entry's weight exceeds rand, return it.
            rand -= entry.spawnWeight;
            if(rand <= 0) return entry;
        }
        
        // Return final valid entry as a failsafe in case something wrong happens.
        return validEntries[^1];
    }
}