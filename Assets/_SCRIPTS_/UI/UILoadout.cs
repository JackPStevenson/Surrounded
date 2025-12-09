using TMPro;
using UnityEngine;

public class UILoadout : MonoBehaviour {
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

    // ------ START METHODS ------
    
    void Start() {
        gridSelect.OnSelected += OnSelected;

        ManagerLoadout.Inst.ClearTaps();
        ManagerLoadout.Inst.AddTap(ManagerData.DefaultTap);
        tapDisplay.SetInfo(ManagerData.DefaultTap);
            
        ManagerLoadout.Inst.ClearSwipes();
        ManagerLoadout.Inst.AddSwipe(ManagerData.DefaultSwipe);
        swipeDisplay.SetInfo(ManagerData.DefaultSwipe);
        
        ManagerLoadout.Inst.ClearShakes();
        ManagerLoadout.Inst.AddShake(ManagerData.DefaultShake);
        shakeDisplay.SetInfo(ManagerData.DefaultShake);
        
        perk1Display.SetInfo("None", "", null);
        perk2Display.SetInfo("None", "", null);
        perk3Display.SetInfo("None", "", null);

        int level = ManagerSaveLoad.GetLevel();
        levelText.text = "Level " + level;
        nextRewardDisplay.SetInfo(ManagerRewards.GetLevelReward(level + 1));
    }

    // ------ UPDATE METHODS ------
    
    void FixedUpdate() {
        bloodText.text = ManagerSaveLoad.GetZombieBlood().ToString("N0");
    }

    // ------ EVENT METHODS ------

    public void StartSelect(int typeId) { if (typeId != 0 && !gridSelect.IsSelecting) gridSelect.Initialize((SelectType) typeId); }

    private void OnSelected(SelectType type, int index) {
        switch (type) {
            case SelectType.WeaponTap:
                DataWeaponTap tap = ManagerData.GetTap(index);
                ManagerLoadout.Inst.ClearTaps();
                ManagerLoadout.Inst.AddTap(tap);
                tapDisplay.SetInfo(tap);
                break;
            
            case SelectType.WeaponSwipe:
                DataWeaponSwipe swipe = ManagerData.GetSwipe(index);
                ManagerLoadout.Inst.ClearSwipes();
                ManagerLoadout.Inst.AddSwipe(swipe);
                swipeDisplay.SetInfo(swipe);
                break;
            
            case SelectType.WeaponShake:
                DataWeaponShake shake = ManagerData.GetShake(index);
                ManagerLoadout.Inst.ClearShakes();
                ManagerLoadout.Inst.AddShake(shake);
                shakeDisplay.SetInfo(shake);
                break;
            
            case SelectType.Perk1:
                SaveLoadFilePerk perk1 = ManagerSaveLoad.GetPerkInstance(index);
                ManagerLoadout.Inst.SetPerkPlayer(perk1, 0);
                perk1Display.SetInfo(perk1.Data);
                break;
            
            case SelectType.Perk2:
                SaveLoadFilePerk perk2 = ManagerSaveLoad.GetPerkInstance(index);
                ManagerLoadout.Inst.SetPerkPlayer(perk2, 1);
                perk2Display.SetInfo(perk2.Data);
                break;
            
            case SelectType.Perk3:
                SaveLoadFilePerk perk3 = ManagerSaveLoad.GetPerkInstance(index);
                ManagerLoadout.Inst.SetPerkPlayer(perk3, 2);
                perk3Display.SetInfo(perk3.Data);
                break;
        }
        
        StopSelect();
    }

    public void StopSelect() {
        gridSelect.Clear();
    }
}
