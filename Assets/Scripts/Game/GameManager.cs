using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public event Action<GameState, int> OnGameStateChanged;
    public static GameManager Instance;
    
    private ZombiesWaveManager _zombiesWaveManager;
    private PoorSoul _poorSoul;

    private GameState _gameState = GameState.Intermission;
    private int _currentWave;
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
        _zombiesWaveManager = ZombiesWaveManager.Instance;
        _zombiesWaveManager.OnAllZombiesDead += OnAllZombiesDead;
        
        _poorSoul = PoorSoul.Instance;
        _poorSoul.OnDeath += OnPlayerDeath;
        _poorSoul.OnDeath += loseMenu.Enable;
        
        _zombiesWaveManager.SetMainTarget(_poorSoul);

        SetGameState(GameState.Intermission);
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
                _currentWave = Mathf.Max(_currentWave + 1, 1);
                _lastIntermission = Time.time;
                break;
            case GameState.InProgress:
                break;
            case GameState.Dead:
                break;
        }
        
        OnGameStateChanged?.Invoke(_gameState, _currentWave);
    }

    // ------ HELPER FUNCTIONS ------
    
    public float GetRemainingIntermission() => Mathf.Max(intermissionTime - (Time.time - _lastIntermission));
    public GameState GetGameState() => _gameState;
    public int GetCurrentWave() => Mathf.Max(_currentWave, 1);
    public PoorSoul GetPoorSoul() => _poorSoul;
}
