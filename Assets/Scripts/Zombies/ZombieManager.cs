using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class ZombieManager : MonoBehaviour {
    public GenericDelegate OnUpdate;
    public IntDelegate OnFixedUpdate;

    public static ZombieManager Instance;

    private ZombiePool _zombiePool;
    private WaveManager _waveManager;

    [Header("References")]
    public GameObject zombieBasePrefab;

    [FormerlySerializedAs("_targetPosDeviation")] [Header("Targeting")]
    public float _approachDistance;
    public LayerMask _sideTargetMask;
    Damageable _mainTarget;

    [Header("Spawning")]
    public Transform[] spawnPoints;
    public ZombieDataTypes[] zombieDataEntries;
    private List<ZombieBase> _activeZombies;
    private int _spawnRandSeed = 0;

    private int _zombieTick;
    
    // ------ START FUNCTIONS ------

    void Awake() {
        _zombieTick = 0;
        Instance = this;
        _zombiePool = new ZombiePool(zombieBasePrefab);
        _activeZombies = new List<ZombieBase>();
    }

    void Start() {
        _waveManager = WaveManager.Instance;
        
        _waveManager.OnHordeSpawn += SpawnZombieHorde;
        _waveManager.OnTrickleSpawn += SpawnZombieRandom;
        _waveManager.OnAllZombiesDead += DepletePool;
    }
    
    // ------ UPDATE FUNCTIONS ------

    void Update() {
        OnUpdate?.Invoke();
    }

    void FixedUpdate() {
        OnFixedUpdate?.Invoke(_zombieTick);
        _zombieTick++;
    }

    // ------ EVENT FUNCTIONS ------

    /// Destroys all zombies in or made from pool.
    public void DepletePool() {
        _zombiePool.DepletePool();
        _activeZombies.Clear();
        
        OnUpdate = null;
        OnFixedUpdate = null;
    }

    // ------ ZOMBIE SPAWNING/RETURNING ------
    
    /// Spawns a zombie with random data from entries based on current wave.
    public void SpawnZombieRandom(int currentWave) {
        GetZombieData(currentWave, out int index);
        SpawnZombieSpecific(index);
    }
    
    /// Spawns a zombie with random data from entries based on current wave and returns spawned zombie.
    public ZombieBase SpawnZombieRandomWithReturn(int currentWave) {
        ZombieDataTypes data = GetZombieData(currentWave, out int index);
        if (!data) return null;

        return SpawnZombieSpecific(index);
    }
    
    /// Spawns a zombie type with data at given data index.
    private ZombieBase SpawnZombieSpecific(int dataIndex) {
        ZombieDataTypes data = zombieDataEntries[Mathf.Clamp(dataIndex, 0, zombieDataEntries.Length - 1)];

        Transform spawn = spawnPoints[Random.Range(0, spawnPoints.Length)];
        ZombieBase z = _zombiePool.Pop(data, spawn.position, _approachDistance, _mainTarget, _sideTargetMask);
        
        OnUpdate += z.UpdateLoop;
        OnFixedUpdate += z.FixedUpdateLoop;
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
    public void ReturnZombie(ZombieBase zombie) {
        OnUpdate -= zombie.UpdateLoop;
        OnFixedUpdate -= zombie.FixedUpdateLoop;

        _zombiePool.Push(zombie);
        _activeZombies.Remove(zombie);
        
    }
    
    // ------ HELPER FUNCTIONS ------

    public ZombieBase[] GetActiveZombies() => _activeZombies.ToArray();
    public int GetActiveZombieCount() => _activeZombies.Count;
    
    /// Attempts to get a random zombie data entry that can be spawned this wave.
    private ZombieDataTypes GetZombieData(int currentWave, out int selectedIndex) {
        selectedIndex = -1;
        float totalWeight = 0;
        List<int> validEntries = new List<int>();
        
        // Find all data entries that can be spawned this wave.
        for (int i = 0; i < zombieDataEntries.Length; i++) {
            ZombieDataTypes types = zombieDataEntries[i];
            
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
            ZombieDataTypes zombie = zombieDataEntries[i];
            
            // If currently examined entry's weight exceeds rand, return it.
            rand -= zombie.spawnWeight;
            if (rand > 0) continue;
                selectedIndex = i;
                return zombie;
        }
        
        // Return final valid entry as a failsafe in case something wrong happens.
        selectedIndex = validEntries.Count - 1;
        return zombieDataEntries[validEntries[^1]];
    }
    
    public void SetMainTarget(Damageable newTarget) => _mainTarget = newTarget;
}