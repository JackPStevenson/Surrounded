using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    
    GameManager _gameManager;
    ZombiesWaveManager _zombiesWaveManager;
    PoorSoul _poorSoul;
    ZombiesManager _zombiesManager;
    
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
        _gameManager = GameManager.Instance;
        _zombiesWaveManager = ZombiesWaveManager.Instance;
        _poorSoul = PoorSoul.Instance;
        _zombiesManager = ZombiesManager.Instance;
        
        _zombiesWaveManager.OnHordeSpawnNotify += HordeSpawnNotify;
    }

    void Update() {
        _healthSlider.value = _poorSoul.HpCurrent / _poorSoul.HpMax;
        switch (_gameManager.GetGameState()) {
            case GameState.Intermission:
                _waveProgressText.text = "Wave " + _gameManager.GetCurrentWave() + " in " + Mathf.Ceil(_gameManager.GetRemainingIntermission());
                _waveProgressSlider.value = _gameManager.GetRemainingIntermission() / _gameManager.intermissionTime;
                break;
            
            case GameState.InProgress:
                _waveProgressText.text = "Wave " + _gameManager.GetCurrentWave();
                _waveProgressSlider.value = _zombiesWaveManager.GetWaveProgress();
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
        yield return new WaitForSeconds(_zombiesWaveManager.hordeStartDelay);
        _HordeText.SetActive(false);
    }
}
