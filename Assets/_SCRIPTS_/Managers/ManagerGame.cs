using System;
using UnityEngine;

public class ManagerGame : MonoBehaviour {
    public event Action<GameState, int> OnGameStateChanged;
    public static ManagerGame Instance;

    private ManagerWave _managerWave;
    private PlayerCore _player;

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
        _managerWave = ManagerWave.Instance;
        _managerWave.OnAllZombiesDead += OnAllZombiesDead;

        _player = PlayerCore.Instance;
        _player.Health.EventDeath += EventPlayerDeath;

        _managerWave.SetMainTarget(_player.Health);

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

    private void OnAllZombiesDead() => SetGameState(GameState.Intermission);
    private void EventPlayerDeath(string deathSource = "") {
        SetGameState(GameState.Dead);
        loseMenu.Enable();
    }

    private void SetGameState(GameState newState) {
        _gameState = newState;


        switch (newState) {
            case GameState.Intermission:
                _currentWave = Mathf.Max(_currentWave + 1, 1);
                PlayerPrefs.SetInt("MaxWaveReached", Mathf.Max(PlayerPrefs.GetInt("MaxWaveReached", 1), _currentWave));
                PlayerPrefs.Save();
                _lastIntermission = Time.time;
                break;
            case GameState.InProgress:
                break;
            case GameState.Dead:
                break;
        }

        OnGameStateChanged?.Invoke(_gameState, _currentWave);
    }

    public void Pause() {
        Time.timeScale = 0;
    }

    public void Resume() {
        Time.timeScale = 1;
    }

    // ------ HELPER FUNCTIONS ------

    public float GetRemainingIntermission() => Mathf.Max(intermissionTime - (Time.time - _lastIntermission));
    public GameState GetGameState() => _gameState;
    public int GetCurrentWave() => Mathf.Max(_currentWave, 1);
    public PlayerCore GetPoorSoul() => _player;
}