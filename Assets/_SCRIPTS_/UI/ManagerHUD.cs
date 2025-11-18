using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManagerHUD : MonoSingleton<ManagerHUD> {

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
    
    protected override void OnAwake() {
        tapDisplay.transform.Find("Slider").TryGetComponent(out _tapEnergySlider);
        swipeDisplay.transform.Find("Slider").TryGetComponent(out _swipeEnergySlider);
        shakeDisplay.transform.Find("Slider").TryGetComponent(out _shakeEnergySlider);
    }

    void Start() {
        ManagerWave.Inst.EventHordeSpawnNotify += HordeSpawnNotify;
        ManagerWeapon.Inst.EventWeaponEquip += OnWeaponEquip;
        
        OnWeaponEquip(ManagerWeapon.Inst.CurrentTap?.Data, 0);
        OnWeaponEquip(ManagerWeapon.Inst.CurrentSwipe?.Data, 1);
        OnWeaponEquip(ManagerWeapon.Inst.CurrentShake?.Data, 2);
    }

    // ------ UPDATE METHODS ------
    
    void Update() {
        healthSlider.value = PlayerCore.Inst.Health.HealthRatio;
        switch (ManagerGame.Inst.GetGameState()) {
            default:
            case GameState.Intermission:
                waveProgressText.text = "Wave " + ManagerGame.Inst.GetCurrentWave() + " in " + Mathf.Ceil(ManagerGame.Inst.GetRemainingIntermission());
                waveProgressSlider.value = ManagerGame.Inst.GetRemainingIntermission() / ManagerGame.Inst.intermissionTime;
                break;

            case GameState.InProgress:
                waveProgressText.text = "Wave " + ManagerGame.Inst.GetCurrentWave();
                waveProgressSlider.value = ManagerWave.Inst.GetWaveProgress();
                break;

            case GameState.Dead:
                break;
        }

        UpdateSliders();
    }

    void UpdateSliders() {
        _tapEnergySlider.value = ManagerWeapon.Inst.CurrentTapEnergy;
        _swipeEnergySlider.value = ManagerWeapon.Inst.CurrentSwipeEnergy;
        _shakeEnergySlider.value = ManagerWeapon.Inst.CurrentShakeEnergy;
    }
    
    // ------ EVENT METHODS ------

    void OnWeaponEquip(DataWeapon weapon, int slot) {
        print(1);
        switch (slot) {
            default:
                tapDisplay.SetIcon(weapon?.icon);
                break;
            case 1:
                swipeDisplay.SetIcon(weapon?.icon);
                break;
            case 2:
                shakeDisplay.SetIcon(weapon?.icon);
                break;
        }
    }
    
    private void HordeSpawnNotify(int intval, bool boolval) => StartCoroutine(DisplayText());

    IEnumerator DisplayText() {
        hordeText.SetActive(true);
        yield return new WaitForSeconds(ManagerWave.Inst.hordeStartDelay);
        hordeText.SetActive(false);
    }
}