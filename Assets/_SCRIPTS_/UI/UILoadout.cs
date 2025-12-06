using System;
using TMPro;
using UnityEngine;

public enum SelectType {
    None,
    WeaponTap,
    WeaponSwipe,
    WeaponShake,
    Perk1,
    Perk2,
    Perk3,
}

public class UILoadout : MonoSingleton<UILoadout> {
    
    [Header("Bundles")]
    public DataBundleMaster masterBundle;
    private DataBundleWeapons Weapons => masterBundle.Weapons;
    private DataBundlePerks Perks => masterBundle.Perks;

    [Header("Weapon Displays")]
    public UIDisplayItem tapDisplay;
    public UIDisplayItem swipeDisplay;
    public UIDisplayItem shakeDisplay;

    [Header("Perk Displays")]
    public UIDisplayItem perk1Display;
    public UIDisplayItem perk2Display;
    public UIDisplayItem perk3Display;
    
    [Header("Selection")]
    public UIGridSelect gridSelect;
    
    [Header("Leveling")]
    public TMP_Text levelText;
    public UIDisplayItemLevelReward nextRewardDisplay;
    
    [Header("Currency")]
    public TMP_Text bloodText;
    

    private SelectType _currentSelectType = SelectType.None;

    // ------ START METHODS ------
    
    protected override void OnAwake() { }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    void Start() {
        gridSelect.OnSelected += OnSelected;

        ManagerLoadout.Inst.ClearTaps();
        ManagerLoadout.Inst.AddTap(masterBundle.defaultTap);
        tapDisplay.SetInfo(masterBundle.defaultTap);
            
        ManagerLoadout.Inst.ClearSwipes();
        ManagerLoadout.Inst.AddSwipe(masterBundle.defaultSwipe);
        swipeDisplay.SetInfo(masterBundle.defaultSwipe);
        
        ManagerLoadout.Inst.ClearShakes();
        ManagerLoadout.Inst.AddShake(masterBundle.defaultShake);
        shakeDisplay.SetInfo(masterBundle.defaultShake);
        
        perk1Display.SetInfo("None", "", null);
        perk2Display.SetInfo("None", "", null);
        perk3Display.SetInfo("None", "", null);

        int level = ManagerSaveLoad.GetLevel();
        levelText.text = "Level " + level;
        nextRewardDisplay.SetInfo(ManagerLevelRewards.GetReward(level + 1));
    }

    void FixedUpdate() {
        bloodText.text = ManagerSaveLoad.GetZombieBlood().ToString("N0");
    }

    // ------ EVENT METHODS ------

    public void StartSelect(int typeId) => StartSelect((SelectType) typeId);
    public void StartSelect(SelectType type) {
        if (type is SelectType.None || _currentSelectType is not SelectType.None) return;
        
        switch (type) {
            case SelectType.WeaponTap: gridSelect.Initialize(Weapons.Taps, "Tap Weapon"); break;
            case SelectType.WeaponSwipe: gridSelect.Initialize(Weapons.Swipes, "Swipe Weapon"); break;
            case SelectType.WeaponShake: gridSelect.Initialize(Weapons.Shakes, "Shake Weapon"); break;
            case SelectType.Perk1: gridSelect.Initialize(Perks.Perks, "Perk 1"); break;
            case SelectType.Perk2: gridSelect.Initialize(Perks.Perks, "Perk 2"); break;
            case SelectType.Perk3: gridSelect.Initialize(Perks.Perks, "Perk 3"); break;
        }
        
        _currentSelectType = type;
    }

    private void OnSelected(int index) {
        
        switch (_currentSelectType) {
            case SelectType.WeaponTap:
                DataWeaponTap tap = Weapons.Taps[index];
                ManagerLoadout.Inst.ClearTaps();
                ManagerLoadout.Inst.AddTap(tap);
                tapDisplay.SetInfo(tap);
                break;
            
            case SelectType.WeaponSwipe:
                DataWeaponSwipe swipe = Weapons.Swipes[index];
                ManagerLoadout.Inst.ClearSwipes();
                ManagerLoadout.Inst.AddSwipe(swipe);
                swipeDisplay.SetInfo(swipe);
                break;
            
            case SelectType.WeaponShake:
                DataWeaponShake shake = Weapons.Shakes[index];
                ManagerLoadout.Inst.ClearShakes();
                ManagerLoadout.Inst.AddShake(shake);
                shakeDisplay.SetInfo(shake);
                break;
            
            case SelectType.Perk1:
                DataPerkPlayer perk1 = Perks.Perks[index];
                ManagerLoadout.Inst.SetPerkPlayer(perk1, 0);
                perk1Display.SetInfo(perk1);
                break;
            
            case SelectType.Perk2:
                DataPerkPlayer perk2 = Perks.Perks[index];
                ManagerLoadout.Inst.SetPerkPlayer(perk2, 1);
                perk2Display.SetInfo(perk2);
                break;
            
            case SelectType.Perk3:
                DataPerkPlayer perk3 = Perks.Perks[index];
                ManagerLoadout.Inst.SetPerkPlayer(perk3, 2);
                perk3Display.SetInfo(perk3);
                break;
        }
        
        StopSelect();
    }

    public void StopSelect() {
        gridSelect.ClearGrid();
        _currentSelectType = SelectType.None;
    }
}
