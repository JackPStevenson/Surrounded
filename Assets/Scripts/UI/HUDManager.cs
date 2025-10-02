using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance;
    
    GameManager _gameManager;
    WaveManager _waveManager;
    PoorSoul _poorSoul;
    ZombieManager _zombieManager;
    
    [Header("HUD Elements")]
    public GameObject _HordeText;
    public Slider _waveProgressSlider;
    public TMP_Text _waveProgressText;

    void Awake() {
        Instance = this;
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _waveManager = WaveManager.Instance;
        _poorSoul = PoorSoul.Instance;
        _zombieManager = ZombieManager.Instance;
        
        _waveManager.OnHordeSpawnNotify += HordeSpawnNotify;
    }

    void Update() {
        switch (_gameManager.GetGameState()) {
            case GameState.Intermission:
                _waveProgressText.text = "Wave " + _gameManager.GetCurrentWave() + " in " + Mathf.Ceil(_gameManager.GetRemainingIntermission());
                _waveProgressSlider.value = _gameManager.GetRemainingIntermission() / _gameManager.intermissionTime;
                break;
            
            case GameState.InProgress:
                _waveProgressText.text = "Wave " + _gameManager.GetCurrentWave();
                _waveProgressSlider.value = _waveManager.GetWaveProgress();
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
        yield return new WaitForSeconds(_waveManager.hordeStartDelay);
        _HordeText.SetActive(false);
    }
}
