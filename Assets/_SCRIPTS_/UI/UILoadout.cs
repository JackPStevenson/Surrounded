using System;
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

public class UILoadout : MonoBehaviour {
    public static UILoadout Instance;

    protected ManagerLoadout Loadout;
    
    [Header("Bundles")]
    public DataBundleMaster masterBundle;
    private DataBundleWeapons TapWeapons => masterBundle.TapWeapons;
    private DataBundleWeapons SwipeWeapons => masterBundle.SwipeWeapons;
    private DataBundleWeapons ShakeWeapons => masterBundle.ShakeWeapons;
    private DataBundlePerks Perks => masterBundle.Perks;

    [Header("General")]
    public UIGridSelect gridSelect;

    [Header("Weapon Displays")]
    public UIDisplayItem tapDisplay;
    public UIDisplayItem swipeDisplay;
    public UIDisplayItem shakeDisplay;

    [Header("Perk Displays")]
    public UIDisplayItem perk1Display;
    public UIDisplayItem perk2Display;
    public UIDisplayItem perk3Display;

    private SelectType _currentSelectType = SelectType.None;

    // ------ START METHODS ------
    
    void Awake() {
        Instance = this;
    }

    void Start() {
        Loadout = ManagerLoadout.Instance;
        gridSelect.OnSelected += OnSelected;

        Loadout.ClearTaps();
        Loadout.AddTap(TapWeapons.FirstTap);
        tapDisplay.SetInfo(TapWeapons.FirstTap);
            
        Loadout.ClearSwipes();
        Loadout.AddSwipe(SwipeWeapons.FirstSwipe);
        swipeDisplay.SetInfo(SwipeWeapons.FirstSwipe);
        
        Loadout.ClearShakes();
        Loadout.AddShake(ShakeWeapons.FirstShake);
        shakeDisplay.SetInfo(ShakeWeapons.FirstShake);

        perk1Display.SetInfo("None", "", null);
        perk2Display.SetInfo("None", "", null);
        perk3Display.SetInfo("None", "", null);
    }

    // ------ EVENT METHODS ------

    public void StartSelect(int typeId) => StartSelect((SelectType) typeId);
    public void StartSelect(SelectType type) {
        if (type is SelectType.None || _currentSelectType is not SelectType.None) return;
        
        switch (type) {
            case SelectType.WeaponTap: gridSelect.Initialize(TapWeapons.Taps, "Tap Weapon"); break;
            case SelectType.WeaponSwipe: gridSelect.Initialize(SwipeWeapons.Swipes, "Swipe Weapon"); break;
            case SelectType.WeaponShake: gridSelect.Initialize(ShakeWeapons.Shakes, "Shake Weapon"); break;
            case SelectType.Perk1: gridSelect.Initialize(Perks.Perks, "Perk 1"); break;
            case SelectType.Perk2: gridSelect.Initialize(Perks.Perks, "Perk 2"); break;
            case SelectType.Perk3: gridSelect.Initialize(Perks.Perks, "Perk 3"); break;
        }
        
        _currentSelectType = type;
    }

    private void OnSelected(int index) {
        
        switch (_currentSelectType) {
            case SelectType.WeaponTap:
                DataWeaponTap tap = TapWeapons.Taps[index];
                Loadout.ClearTaps();
                Loadout.AddTap(tap);
                tapDisplay.SetInfo(tap);
                break;
            
            case SelectType.WeaponSwipe:
                DataWeaponSwipe swipe = SwipeWeapons.Swipes[index];
                Loadout.ClearSwipes();
                Loadout.AddSwipe(swipe);
                swipeDisplay.SetInfo(swipe);
                break;
            
            case SelectType.WeaponShake:
                DataWeaponShake shake = ShakeWeapons.Shakes[index];
                Loadout.ClearShakes();
                Loadout.AddShake(shake);
                shakeDisplay.SetInfo(shake);
                break;
            
            case SelectType.Perk1:
                DataPerkPlayer perk1 = Perks.Perks[index];
                Loadout.SetPerkPlayer(perk1, 0);
                perk1Display.SetInfo(perk1);
                break;
            
            case SelectType.Perk2:
                DataPerkPlayer perk2 = Perks.Perks[index];
                Loadout.SetPerkPlayer(perk2, 1);
                perk2Display.SetInfo(perk2);
                break;
            
            case SelectType.Perk3:
                DataPerkPlayer perk3 = Perks.Perks[index];
                Loadout.SetPerkPlayer(perk3, 2);
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
