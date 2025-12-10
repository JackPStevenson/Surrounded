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
    public UIItemPreview itemPreview;
    
    [Header("Leveling")]
    public TMP_Text levelText;
    public UIDisplayItemLevelReward nextRewardDisplay;
    
    [Header("Shop")]
    public TMP_Text currentBlood;
    public TMP_Text perkBloodCost;
    

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
        nextRewardDisplay.SetInfo(ManagerRewards.GetLevelReward(level + 1), level + 1);
        nextRewardDisplay.SetDesc((level + 1) + "");
    }

    // ------ UPDATE METHODS ------
    
    void FixedUpdate() {
        
        currentBlood.text = ManagerSaveLoad.GetZombieBlood().ToString("N0");
    }

    // ------ EVENT METHODS ------

    public void StartSelect(int typeId) {
        if (typeId == 0 || gridSelect.IsSelecting) return;

        switch (typeId) {
            case 4:
                gridSelect.Initialize((SelectType) typeId, new IDisplayable[] { ManagerLoadout.Inst.GetPerk(1), ManagerLoadout.Inst.GetPerk(2) });
                break;
            case 5:
                gridSelect.Initialize((SelectType) typeId, new IDisplayable[] { ManagerLoadout.Inst.GetPerk(0), ManagerLoadout.Inst.GetPerk(2) });
                break;
            case 6:
                gridSelect.Initialize((SelectType) typeId, new IDisplayable[] { ManagerLoadout.Inst.GetPerk(0), ManagerLoadout.Inst.GetPerk(1) });
                break;
            default:
                gridSelect.Initialize((SelectType) typeId);
                break;
        }
    }

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
                SaveLoadPerk perk1 = ManagerSaveLoad.GetPerkInstance(index);
                ManagerLoadout.Inst.SetPerkPlayer(perk1, 0);
                perk1Display.SetInfoFromPerk(perk1, false);
                break;
            
            case SelectType.Perk2:
                SaveLoadPerk perk2 = ManagerSaveLoad.GetPerkInstance(index);
                ManagerLoadout.Inst.SetPerkPlayer(perk2, 1);
                perk2Display.SetInfoFromPerk(perk2, false);
                break;
            
            case SelectType.Perk3:
                SaveLoadPerk perk3 = ManagerSaveLoad.GetPerkInstance(index);
                ManagerLoadout.Inst.SetPerkPlayer(perk3, 2);
                perk3Display.SetInfoFromPerk(perk3, false);
                break;
        }
        
        StopSelect();
    }

    public void StopSelect() => gridSelect.Clear();
    public void TryBuyPerk() {
        if (!ManagerRewards.TryBuyPerk(out SaveLoadPerk perk)) return;
        
        itemPreview.transform.parent.gameObject.SetActive(true);
        itemPreview.PreviewItem(perk);
    }
}