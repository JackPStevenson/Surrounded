using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    Intermission,
    InProgress,
    Dead
}

public class GameManager : MonoBehaviour {
    public delegate void GameStateDelegate(GameState gameState);
    public GameStateDelegate OnGameStateChanged;
    
    public static GameManager Instance;
    private ZombieManager _zombieManager;
    private GameState _gameState = GameState.Intermission;

    
    [Header("General")]
    public float intermissionTime = 5;
    private int _currentWave = 1;
    
    
    
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
}
