using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerHUD : MonoBehaviour
{
    public static ManagerHUD Instance;
    
    ManagerGame _managerGame;
    ManagerWave _managerWave;
    PlayerCore _player;
    ManagerZombies _managerZombies;
    
    [Header("HUD Elements")]
    public GameObject _HordeText;
    public Slider _waveProgressSlider;
    public TMP_Text _waveProgressText;
    public Slider _healthSlider;

    void Awake() {
        Instance = this;
    }

    void Start()
    {
        _managerGame = ManagerGame.Instance;
        _managerWave = ManagerWave.Instance;
        _player = PlayerCore.Instance;
        _managerZombies = ManagerZombies.Instance;
        
        _managerWave.EventHordeSpawnNotify += HordeSpawnNotify;
    }

    void Update() {
        _healthSlider.value = _player.Health.HealthRatio;
        switch (_managerGame.GetGameState()) {
            case GameState.Intermission:
                _waveProgressText.text = "Wave " + _managerGame.GetCurrentWave() + " in " + Mathf.Ceil(_managerGame.GetRemainingIntermission());
                _waveProgressSlider.value = _managerGame.GetRemainingIntermission() / _managerGame.intermissionTime;
                break;
            
            case GameState.InProgress:
                _waveProgressText.text = "Wave " + _managerGame.GetCurrentWave();
                _waveProgressSlider.value = _managerWave.GetWaveProgress();
                break;
            
            case GameState.Dead:
                break;
        }
    }
    
    private void HordeSpawnNotify(int intval, bool boolval) {
        StartCoroutine(DisplayText());
    }

    IEnumerator DisplayText() {
        _HordeText.SetActive(true);
        yield return new WaitForSeconds(_managerWave.hordeStartDelay);
        _HordeText.SetActive(false);
    }
}
