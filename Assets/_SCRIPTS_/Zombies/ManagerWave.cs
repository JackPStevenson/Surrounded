using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class ManagerWave : MonoSingleton<ManagerWave> {
    public event Action<int> EventTrickleSpawn;
    public event Action<int, bool> EventHordeSpawnNotify; // True if horde is last one of this wave.
    public event Action<int, int, float> OnHordeSpawn;
    public event Action OnAllZombiesDead;

    private Health _mainTarget;
    
    private int _currentWave = 1;

    [Header("General")]
    public float startingWaveDuration = 10;
    public float waveDurationRampPerWave = 2.5f;
    public bool _isSpawning = false;
    public bool _isWaveCompleted = true;

    [Header("Trickle")]
    public float startingTrickleSpawnDelay = 1.5f;
    /// How much (as percentage) faster trickle delay should be relative to last wave.
    [Range(0, 100)]
    public float trickleRatePercentIncreasePerWave = 11f;
    [Min(0)]
    public float trickleRateDelayMinimum = 0.25f;

    [Header("Hordes")]
    public int startingHordeCount = 10;
    public int hordeCountRampPerWave = 4;
    public float minimumWaveDurationPerHorde = 10;
    [Space]
    public float hordeStartDelay = 1.5f;
    public float hordeIndividualZombieSpawnDelay = 0.1f;

    private float _waveStartTime;
    private float _lastTrickleSpawn;
    private int _hordesSpawned = 0;

    // ------ START FUNCTIONS ------
    
    protected override void OnAwake() { }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    void Start() {
        ManagerGame.Inst.OnGameStateChanged += OnManagerGameStateChanged;
        
        ToggleWave(false);
    }

    // ------ UPDATE FUNCTIONS ------

    void FixedUpdate() {
        // Failsafe.
        if (_isWaveCompleted) return;

        // Calculate current wave duration and total number of hordes this wave.
        float currentWaveDuration = GetWaveDuration();
        int hordesThisWave = Mathf.FloorToInt(currentWaveDuration / startingWaveDuration);
        
        // Calculate delay between each horde and time each horde will take to spawn all its zombies.
        float delayBetweenHordes = currentWaveDuration / hordesThisWave;
        float hordeTimeToFullySpawn = hordeStartDelay + (GetHordeSize() * hordeIndividualZombieSpawnDelay);

        // If spawning is still enabled and time for another trickle spawn elapses, spawn a trickle zombie.
        if (_isSpawning && _lastTrickleSpawn + GetTrickleSpawnDelay() <= Time.time ) {
            EventTrickleSpawn?.Invoke(_currentWave);
            
            _lastTrickleSpawn = Time.time;
        }

        // Spawn a horde if spawning is still active and enough time has elapsed between now and last one.
        if (_isSpawning && _waveStartTime + (delayBetweenHordes * (_hordesSpawned + 1)) <= Time.time) {
            _hordesSpawned++;
            
            // Calculate whether this is last horde, then spawn it.
            bool isLastHorde = _hordesSpawned >= hordesThisWave;
            StartCoroutine(SpawnHordeDelayed(isLastHorde));

            // If this is last horde, stop spawning.
            if (isLastHorde) _isSpawning = false;
        }

        // Calculate when final horde will fully spawn.
        if (_waveStartTime + currentWaveDuration + hordeTimeToFullySpawn <= Time.time) {
            // If all zombies are dead, send a message through OnAllZombiesDead delegate.
            if (ManagerZombies.Inst.ActiveZombieCount <= 0) {
                ToggleWave(false);
                OnAllZombiesDead?.Invoke();
            }
        }
    }

    // ------ EVENT FUNCTIONS ------

    private IEnumerator SpawnHordeDelayed(bool isLastHorde) {
        EventHordeSpawnNotify?.Invoke(_currentWave, isLastHorde);

        yield return new WaitForSeconds(hordeStartDelay);

        OnHordeSpawn?.Invoke(_currentWave, GetHordeSize(), hordeIndividualZombieSpawnDelay);
    }
    
    private void OnManagerGameStateChanged(GameState gameState, int currentWave) {
        _currentWave = currentWave;
        
        ToggleWave(gameState == GameState.InProgress);
    }

    public void ToggleWave(bool isEnabled) {
        enabled = isEnabled;
        
        _isSpawning = isEnabled;
        _isWaveCompleted = !isEnabled;

        if (isEnabled) {
            _waveStartTime = Time.time;
            _hordesSpawned = 0;
        }
    }
    
    // ------ HELPER FUNCTIONS ------

    private float GetWaveDuration() {
        return startingWaveDuration + (waveDurationRampPerWave * (_currentWave - 1));
    }

    private int GetHordeSize() {
        return startingHordeCount + (hordeCountRampPerWave * (_currentWave - 1));
    }

    private float GetTrickleSpawnDelay() {
        // Calculate how much shorter current spawn delay should be relative to starting spawn delay.
        float percentToBase = 1 + (trickleRatePercentIncreasePerWave / 100);
        float expInv = Mathf.Pow(1 / percentToBase, _currentWave);
        
        // Make sure new delay approaches but never reaches given minimum delay.
        float newDelay = (startingTrickleSpawnDelay - trickleRateDelayMinimum) * expInv;
        return newDelay + trickleRateDelayMinimum;
    }
    
    public float GetHordeSpawnTime() => hordeStartDelay + (GetHordeSize() * hordeIndividualZombieSpawnDelay);
    
    public Health GetMainTarget() => _mainTarget;
    
    public void SetMainTarget(Health mainTarget) {
        _mainTarget = mainTarget;
        ManagerZombies.Inst.SetMainTarget(_mainTarget);
    }

    public float GetWaveProgress() => Mathf.Clamp01((Time.time - _waveStartTime) / (GetWaveDuration() + GetHordeSpawnTime()));
}