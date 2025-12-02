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

    // ------ START METHODS ------

    // ------ EVENT METHODS ------

    public void Toggle(bool active) {
        if (active == gameObject.activeSelf) return;

        bool noRewardsToShow = onlyRewardsToBeRewarded && !ManagerSaveLoad.CheckRewardsForPlayer();
        if (active && !noRewardsToShow) {
            CreateDisplayItems();

            levelText.text = "Level " + ManagerSaveLoad.GetLevel();
            scroller.horizontalNormalizedPosition = 0;
        }
        else {
            for (int i = ScrollerContent.childCount - 1; i >= 0; i--)
                Destroy(ScrollerContent.GetChild(i).gameObject);
        }
        print(ScrollerContent.childCount);

        gameObject.SetActive(active);
    }

    private void CreateDisplayItems() {
        DataDisplayable[] rewards = onlyRewardsToBeRewarded
            ? ManagerLevelRewards.Inst.GetRewardsToBeRewarded()
            : ManagerLevelRewards.Inst.GetRewardsInLevelRange(1, maxLevelToDisplay);

        for (int i = 0; i < rewards.Length; i++) {
            Instantiate(levelRewardPrefab, ScrollerContent).TryGetComponent(out UIDisplayItemLevelReward rewardDisplay);

            int rewardLevel = (i + 1) + (onlyRewardsToBeRewarded ? ManagerSaveLoad.GetLastRewardedLevel() : 0);
            rewardDisplay.SetInfo(rewardLevel, rewards[i].icon);
        }
    }
}