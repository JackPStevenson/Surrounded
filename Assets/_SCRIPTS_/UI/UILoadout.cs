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
    private DataBundleWeapons Weapons => masterBundle.BundleWeapons;
    private DataBundlePerksPlayer PerksPlayer => masterBundle.BundlePerksPlayer;

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
        Loadout.AddTap(Weapons.DefaultTap);
        tapDisplay.SetInfo(Weapons.DefaultTap);
            
        Loadout.ClearSwipes();
        Loadout.AddSwipe(Weapons.DefaultSwipe);
        swipeDisplay.SetInfo(Weapons.DefaultSwipe);
        
        Loadout.ClearShakes();
        Loadout.AddShake(Weapons.DefaultShake);
        shakeDisplay.SetInfo(Weapons.DefaultShake);

        perk1Display.SetInfo("None", "", null);
        perk2Display.SetInfo("None", "", null);
        perk3Display.SetInfo("None", "", null);
    }

    // ------ EVENT METHODS ------

    public void StartSelect(int typeId) => StartSelect((SelectType) typeId);
    public void StartSelect(SelectType type) {
        if (type is SelectType.None || _currentSelectType is not SelectType.None) return;
        
        switch (type) {
            case SelectType.WeaponTap: gridSelect.Initialize(Weapons.GetItems<DataWeaponTap>(), "Tap Weapon"); break;
            case SelectType.WeaponSwipe: gridSelect.Initialize(Weapons.GetItems<DataWeaponSwipe>(), "Swipe Weapon"); break;
            case SelectType.WeaponShake: gridSelect.Initialize(Weapons.GetItems<DataWeaponShake>(), "Shake Weapon"); break;
            case SelectType.Perk1: gridSelect.Initialize(PerksPlayer.PlayerPerks.ToArray(), "Perk 1"); break;
            case SelectType.Perk2: gridSelect.Initialize(PerksPlayer.PlayerPerks.ToArray(), "Perk 2"); break;
            case SelectType.Perk3: gridSelect.Initialize(PerksPlayer.PlayerPerks.ToArray(), "Perk 3"); break;
        }
        
        _currentSelectType = type;
    }

    private void OnSelected(int index) {
        
        switch (_currentSelectType) {
            case SelectType.WeaponTap:
                DataWeaponTap tap = Weapons.GetItems<DataWeaponTap>()[index];
                Loadout.ClearTaps();
                Loadout.AddTap(tap);
                tapDisplay.SetInfo(tap);
                break;
            
            case SelectType.WeaponSwipe:
                DataWeaponSwipe swipe = Weapons.GetItems<DataWeaponSwipe>()[index];
                Loadout.ClearSwipes();
                Loadout.AddSwipe(swipe);
                swipeDisplay.SetInfo(swipe);
                break;
            
            case SelectType.WeaponShake:
                DataWeaponShake shake = Weapons.GetItems<DataWeaponShake>()[index];
                Loadout.ClearShakes();
                Loadout.AddShake(shake);
                shakeDisplay.SetInfo(shake);
                break;
            
            case SelectType.Perk1:
                DataPerkPlayer perk1 = PerksPlayer.PlayerPerks[index];
                Loadout.SetPerkPlayer(perk1, 0);
                perk1Display.SetInfo(perk1);
                break;
            
            case SelectType.Perk2:
                DataPerkPlayer perk2 = PerksPlayer.PlayerPerks[index];
                Loadout.SetPerkPlayer(perk2, 1);
                perk2Display.SetInfo(perk2);
                break;
            
            case SelectType.Perk3:
                DataPerkPlayer perk3 = PerksPlayer.PlayerPerks[index];
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
