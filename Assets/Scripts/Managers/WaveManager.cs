using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;


public class WaveManager : MonoBehaviour {
    public IntDelegate OnTrickleSpawn;
    /// boolVal is true if horde is last one of this wave.
    public IntBoolDelegate OnHordeSpawn;

    public static WaveManager Instance;
    private ZombieManager _zombieManager;

    private int _currentWave;

    [Header("General")]
    public float startingWaveDuration = 10;
    public float waveDurationRampPerWave = 2.5f;
    private bool _spawningCompleted = false;

    [Header("Trickle")]
    public float startingTrickleSpawnDelay = 1.5f;
    public float trickleSpawnDelayRampPerWave = 0.01f;

    [Header("Hordes")]
    public int startingHordeCount = 10;
    public int hordeCountRampPerWave = 4;
    public float minimumWaveDurationPerHorde = 10;
    [Space]
    public float hordeStartDelay = 1.5f;
    
    private float _waveStartTime;
    private float _lastTrickleSpawn;
    private int _hordesSpawned = 0;

    private void Awake() {
        Instance = this;
    }

    void Start() {
        _zombieManager = ZombieManager.Instance;
    }

    void FixedUpdate() {
        if (_spawningCompleted) return;

        // Calculate total number of hordes this wave the time between them.
        float currentWaveDuration = GetWaveDuration();
        int hordesThisWave = Mathf.FloorToInt(currentWaveDuration / startingWaveDuration);
        float hordeSpawnDelay = currentWaveDuration / hordesThisWave;

        
        
        if (_lastTrickleSpawn + startingTrickleSpawnDelay <= Time.time) {
            OnTrickleSpawn?.Invoke(_currentWave);
        }

        // Spawn a horde when enough time has elapsed.
        if (_waveStartTime + (hordeSpawnDelay * (_hordesSpawned + 1)) <= Time.time) {
            bool isLastHorde = _waveStartTime + currentWaveDuration <= Time.time;
            StartCoroutine(SpawnHordeDelayed(isLastHorde));
            
            if (_waveStartTime + currentWaveDuration <= Time.time)
                ToggleSpawning(_currentWave, false);
        }
    }

    private IEnumerator SpawnHordeDelayed(bool isLastHorde) {
        yield return new WaitForSeconds(hordeStartDelay);
        OnHordeSpawn?.Invoke(_currentWave, isLastHorde);
    }

    private float GetWaveDuration() {
        return startingWaveDuration + (waveDurationRampPerWave * (_currentWave - 1));
    }

    public void ToggleSpawning(int currentWave, bool isEnabled) {
        _currentWave = currentWave;
        enabled = isEnabled;

        _spawningCompleted = !isEnabled;

        if (isEnabled) {
            _waveStartTime = Time.time;
            _hordesSpawned = 0;
        }
    }
}