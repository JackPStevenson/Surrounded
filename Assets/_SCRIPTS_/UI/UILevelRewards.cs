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
    public bool onlyRewardsToBeRewarded = false;
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
    
    public void Toggle(bool active) => Toggle(active, false);
    public void Toggle(bool active, bool forceToggle) {
        if (active == gameObject.activeSelf && !forceToggle) return;

        bool noRewardsToShow = onlyRewardsToBeRewarded && !ManagerSaveLoad.CheckForRewards();
        if (active && !noRewardsToShow) {
            CreateDisplayItems();

            if(levelText) levelText.text = "Level " + ManagerSaveLoad.GetLevel();
            scroller.horizontalNormalizedPosition = 0;
        }
        else {
            for (int i = ScrollerContent.childCount - 1; i >= 0; i--)
                Destroy(ScrollerContent.GetChild(i).gameObject);
        }

        gameObject.SetActive(active);
    }

    private void CreateDisplayItems() {
        DataDisplayable[] rewards = onlyRewardsToBeRewarded
            ? ManagerLevelRewards.GetUnrewardedRewards()
            : ManagerLevelRewards.GetRewards(1, maxLevelToDisplay);

        for (int i = 0; i < rewards.Length; i++) {
            Instantiate(levelRewardPrefab, ScrollerContent).TryGetComponent(out UIDisplayItemLevelReward rewardDisplay);

            int rewardLevel = (i + 1) + (onlyRewardsToBeRewarded ? ManagerSaveLoad.GetLastRewardLevel() : 0);
            if (rewards[i] is DataLevelGenericReward) {
                rewardDisplay.SetInfo(rewards[i], rewardLevel);
                rewardDisplay.SetName("Blood (" + ManagerLevelRewards.Inst.bloodRewardAmount + ")");
            }
            else {
                rewardDisplay.SetInfo(rewards[i], rewardLevel);
            }
        }
    }
}