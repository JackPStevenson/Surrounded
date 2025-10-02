using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameStateIntDelegate OnGameStateChanged;
    public static GameManager Instance;
    
    private WaveManager _waveManager;
    private PoorSoul _poorSoul;

    private GameState _gameState = GameState.Intermission;
    private int _currentWave = 1;
    private float _lastIntermission;

    [Header("General")]
    public MenuBase loseMenu;
    
    // Parameters
    [Header("General")]
    public float intermissionTime = 5;
    
    
    // ------ START FUNCTIONS ------

    void Awake() {
        Instance = this;
    }

    void Start() {
        _waveManager = WaveManager.Instance;
        _waveManager.OnAllZombiesDead += OnAllZombiesDead;
        
        _poorSoul = PoorSoul.Instance;
        _poorSoul.OnDeath += OnPlayerDeath;
        _poorSoul.OnDeath += loseMenu.Enable;
        
        _waveManager.SetMainTarget(_poorSoul);
    }

    // ------ UPDATE FUNCTIONS ------

    void Update() {
        
    }

    void FixedUpdate() {
        if (_gameState is not GameState.Intermission) return;

        if (_lastIntermission + intermissionTime < Time.time) SetGameState(GameState.InProgress);
    }
    
    // ------ EVENT FUNCTIONS ------

    private void OnAllZombiesDead() {
        SetGameState(GameState.Intermission);
    }
    
    private void OnPlayerDeath() {
        SetGameState(GameState.Dead);
    }
    
    private void SetGameState(GameState newState) {
        _gameState = newState;
        
        switch (newState) {
            case GameState.Intermission:
                _lastIntermission = Time.time;
                break;
            case GameState.InProgress:
                _currentWave++;
                break;
            case GameState.Dead:
                break;
        }
        
        OnGameStateChanged?.Invoke(_gameState, _currentWave);
    }

    // ------ HELPER FUNCTIONS ------
    
    public GameState GetGameState() => _gameState;
    public int GetCurrentWave() => _currentWave;
    public PoorSoul GetPoorSoul() => _poorSoul;
}
