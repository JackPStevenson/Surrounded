using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UILevelRewards : MonoBehaviour {
    [Header("References")]
    public TMP_Text levelText;
    public ScrollRect scroller;
    private Transform ScrollerContent => scroller.content;

    [Header("Display")]
    public GameObject levelRewardPrefab;
    public int maxLevelToDisplay = 50;
    public bool onlyPending = false;
    public bool displayOnEnable = false;

    // ------ START METHODS ------

    // ------ EVENT METHODS ------

    private void OnEnable() {
        if (displayOnEnable)
            Toggle(true, displayOnEnable);
    }

    private void OnDisable() {
        if (displayOnEnable)
            Toggle(false);
    }

    public void Toggle(bool active) => Toggle(active, true, false);
    public void Toggle(bool active, bool autoInitialize) => Toggle(active, autoInitialize, false);
    public void Toggle(bool active, bool autoInitialize, bool forceToggle) {
        print(active);
        if (active == gameObject.activeSelf && !forceToggle) return;
        bool noRewardsToShow = onlyPending && !ManagerSaveLoad.CheckForRewards();
        if (active && !noRewardsToShow) {
            if (autoInitialize) CreateRewardDisplays();
            if (levelText) levelText.text = "Level " + ManagerSaveLoad.GetLevel();
            scroller.horizontalNormalizedPosition = 0;
        }
        else
            for (int i = ScrollerContent.childCount - 1; i >= 0; i--)
                Destroy(ScrollerContent.GetChild(i).gameObject);

        gameObject.SetActive(active);
    }
    
    // ------ DISPLAY CREATION METHODS ------

    private void CreateRewardDisplays() {
        for (int i = (onlyPending ? ManagerSaveLoad.GetLastRewardLevel() : 0); i < (onlyPending ? ManagerSaveLoad.GetLevel() : maxLevelToDisplay); i++)
            CreateSingleRewardDisplay(i + 1);
    }

    public void CreateSingleRewardDisplay(int rewardLevel) {
        DataDisplayable reward = ManagerRewards.GetLevelReward(rewardLevel);
        
        Instantiate(levelRewardPrefab, ScrollerContent).TryGetComponent(out UIDisplayItemLevelReward rewardDisplay);
        rewardDisplay.SetInfo(reward, rewardLevel);

        if (reward is DataLevelGenericReward) rewardDisplay.SetName("Blood (" + ManagerRewards.BloodLevelRewardAmount + ")");
    }
}