using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public GameStateDelegate OnGameStateChanged;
    public IntDelegate OnWaveStart;
    public IntDelegate OnWaveComplete;
    
    public static GameManager Instance;
    private ZombieManager _zombieManager;
    private GameState _gameState = GameState.Intermission;

    private int _currentWave = 1;
    
    // Parameters
    [Header("General")]
    public float intermissionTime = 5;
    
    
    // ------ START FUNCTIONS ------

    void Awake() {
        Instance = this;
    }

    void Start() {
        _zombieManager = ZombieManager.Instance;
    }

    // ------ UPDATE FUNCTIONS ------

    void Update() {
        
    }

    void FixedUpdate() {
        
    }
    
    // ------ EVENT FUNCTIONS ------
    
    private void SetGameState(GameState newState) {
        _gameState = newState;
        OnGameStateChanged?.Invoke(_gameState);
    }

    // ------ HELPER FUNCTIONS ------
    
    public GameState GetGameState() => _gameState;
    public int GetCurrentWave() => _currentWave;
}
