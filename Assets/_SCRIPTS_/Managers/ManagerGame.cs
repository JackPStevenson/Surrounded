using System;
using UnityEngine;

public class ManagerGame : MonoSingleton<ManagerGame> {
    public event Action<GameState, int> OnGameStateChanged;
    public static ManagerGame Instance;

    private GameState _gameState = GameState.Intermission;
    private int _currentWave;
    private float _lastIntermission;
    
    [Header("General")]
    public float intermissionTime = 5;
    
    [Header("UI")]
    public UILayoutManager hudLayout;
    public int gameOverCanvasIndex;


    // ------ START FUNCTIONS ------

    protected override void OnAwake() { }

    void Start() {
        PlayerCore.Inst.Health.EventDeath += EventPlayerDeath;
        
        ManagerWave.Inst.OnAllZombiesDead += OnAllZombiesDead;
        ManagerWave.Inst.SetMainTarget(PlayerCore.Inst.Health);

        SetGameState(GameState.Intermission);
    }
    
    protected override void OnDestroyed(bool isDeletedInstance) { }

    // ------ UPDATE FUNCTIONS ------

    void FixedUpdate() {
        if (_gameState is not GameState.Intermission) return;
        if (_lastIntermission + intermissionTime < Time.time) SetGameState(GameState.InProgress);
    }

    // ------ EVENT FUNCTIONS ------

    private void OnAllZombiesDead() => SetGameState(GameState.Intermission);
    private void EventPlayerDeath(string deathSource = "") {
        SetGameState(GameState.Dead);
        hudLayout.SwitchCanvas(gameOverCanvasIndex);
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
}