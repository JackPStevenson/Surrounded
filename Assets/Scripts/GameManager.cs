using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState {
    Menu,
    GameIntermission,
    GameInProgress,
    GameEnded
}

public class GameManager : MonoBehaviour {
    public static GameManager Instance;
    private ZombieManager _zombieManager;
    
    private GameState _gameState = GameState.Menu;
    
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

    // ------ HELPER FUNCTIONS ------
    
    public GameState GetGameState() => _gameState;
}
