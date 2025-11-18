using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerHUD : MonoBehaviour {
    public static ManagerHUD Instance;

    ManagerGame _managerGame;
    ManagerWave _managerWave;
    ManagerWeapon _managerWeapon;
    PlayerCore _player;
    ManagerZombies _managerZombies;

    [Header("HUD Elements")]
    public GameObject hordeText;
    public Slider waveProgressSlider;
    public TMP_Text waveProgressText;
    public Slider healthSlider;

    [Header("Weapons")]
    public UIDisplayItem tapDisplay;
    private Slider _tapEnergySlider;
    public UIDisplayItem swipeDisplay;
    private Slider _swipeEnergySlider;
    public UIDisplayItem shakeDisplay;
    private Slider _shakeEnergySlider;
    
    // ------ START METHODS ------

    void Awake() {
        Instance = this;
        
        tapDisplay.transform.Find("Slider").TryGetComponent(out _tapEnergySlider);
        swipeDisplay.transform.Find("Slider").TryGetComponent(out _swipeEnergySlider);
        shakeDisplay.transform.Find("Slider").TryGetComponent(out _shakeEnergySlider);
    }

    void Start() {
        _managerGame = ManagerGame.Instance;
        _managerWave = ManagerWave.Instance;
        _player = PlayerCore.Instance;
        _managerZombies = ManagerZombies.Instance;
        _managerWeapon = ManagerWeapon.Instance;

        _managerWave.EventHordeSpawnNotify += HordeSpawnNotify;
        _managerWeapon.EventWeaponEquip += OnWeaponEquip;
    }

    // ------ UPDATE METHODS ------
    
    void Update() {
        healthSlider.value = _player.Health.HealthRatio;
        switch (_managerGame.GetGameState()) {
            default:
            case GameState.Intermission:
                waveProgressText.text = "Wave " + _managerGame.GetCurrentWave() + " in " + Mathf.Ceil(_managerGame.GetRemainingIntermission());
                waveProgressSlider.value = _managerGame.GetRemainingIntermission() / _managerGame.intermissionTime;
                break;

            case GameState.InProgress:
                waveProgressText.text = "Wave " + _managerGame.GetCurrentWave();
                waveProgressSlider.value = _managerWave.GetWaveProgress();
                break;

            case GameState.Dead:
                break;
        }

        _tapEnergySlider.value = (_managerWeapon.CurrentTap) ? _managerWeapon.CurrentTap.CurrentEnergy : 0;
        _swipeEnergySlider.value = (_managerWeapon.CurrentSwipe) ? _managerWeapon.CurrentSwipe.CurrentEnergy : 0;
        _shakeEnergySlider.value = (_managerWeapon.CurrentShake) ? _managerWeapon.CurrentShake.CurrentEnergy : 0;
    }
    
    // ------ EVENT METHODS ------

    void OnWeaponEquip(DataWeapon weapon, int slot) {
        switch (slot) {
            default:
                tapDisplay.SetIcon(weapon.icon);
                break;
            case 1:
                swipeDisplay.SetIcon(weapon.icon);
                break;
            case 2:
                shakeDisplay.SetIcon(weapon.icon);
                break;
        }
    }
    
    private void HordeSpawnNotify(int intval, bool boolval) => StartCoroutine(DisplayText());

    IEnumerator DisplayText() {
        hordeText.SetActive(true);
        yield return new WaitForSeconds(_managerWave.hordeStartDelay);
        hordeText.SetActive(false);
    }
}