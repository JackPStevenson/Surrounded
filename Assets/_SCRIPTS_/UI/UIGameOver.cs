using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIGameOver : MonoBehaviour {
    [Header("General")]
    public float microDelay = 0.4f;
    public float macroDelay = 1.0f;
    
    [Header("Performance Panel")]
    public GameObject performanceStatPrefab;
    public GameObject performancePanelObject;
    [Space]
    public UIPerformanceStatDisplay wavesDisplay;
    public Transform genericSubDisplayHolder;
    [Space]
    public UIPerformanceStatDisplay totalZombiesDisplay;
    public Transform zombiesSubDisplayHolder;
    [Space]
    public UIPerformanceStatDisplay totalEarningsDisplay;
    
    [Header("Level Panel")]
    public GameObject levelPanelObject;
    [Space]
    public float levelProgressTime = 3;
    public TMP_Text currentLevelText;
    public TMP_Text nextLevelText;
    public Slider baseLevelSlider;
    public Slider expGainedSlider;
    public UIDisplayItemLevelReward nextLevelReward;

    [Header("Rewards Panel")]
    public GameObject rewardsPanelObject;
    [Space]
    public UILevelRewards levelRewards;

    public void StartDisplay(UIStat wave, UIStat[] generic, UIStat totalZombieKills, UIStat[] zombieKills, UIStat totalEarnings, int startLevel, int startExp) {
        
        StartCoroutine(DoPerformancePanel(wave, generic, totalZombieKills, zombieKills, totalEarnings, startLevel, startExp));
    }

    IEnumerator DoPerformancePanel(UIStat wave, UIStat[] generic, UIStat totalZombieKills, UIStat[] zombieKills, UIStat totalEarnings, int startLevel, int startExp) {
        UIPerformanceStatDisplay temp;
        
        // Show performance panel after delay.
        yield return new WaitForSeconds(macroDelay);
        performancePanelObject.SetActive(true);
        
        // Show wave stat after delay. After, display each generic sub-stat.
        yield return new WaitForSeconds(microDelay);
        wavesDisplay.SetInfo(wave);
        foreach (UIStat genericStat in generic) {
            yield return new WaitForSeconds(microDelay);
            Instantiate(performanceStatPrefab, genericSubDisplayHolder).TryGetComponent(out temp);
            temp.SetInfo(genericStat);
        }
        
        // Show total zombies stat after delay. After, display each zombie sub-stat.
        yield return new WaitForSeconds(microDelay);
        totalZombiesDisplay.SetInfo(totalZombieKills);
        foreach (UIStat zombieKillStat in generic) {
            yield return new WaitForSeconds(microDelay);
            Instantiate(performanceStatPrefab, zombiesSubDisplayHolder).TryGetComponent(out temp);
            temp.SetInfo(zombieKillStat);
        }
        
        // Show total earnings. After, start level panel display.
        yield return new WaitForSeconds(microDelay);
        totalEarningsDisplay.SetInfo(totalEarnings);
        StartCoroutine(DoLevelPanel(startLevel, startExp));
    }

    IEnumerator DoLevelPanel(int startLevel, int startExp) {
        // Show level panel after delay.
        yield return new WaitForSeconds(macroDelay);
        levelPanelObject.SetActive(true);
        
        yield return new WaitForSeconds(microDelay);
        currentLevelText.text = startLevel.ToString("N0");
        nextLevelText.text = (startLevel + 1).ToString("N0");

        float startProgress = ((float) startExp / ManagerSaveLoad.GetExperiencePerLevel());
        float targetLevelAndProgress = ManagerSaveLoad.GetLevel() + ManagerSaveLoad.GetLevelProgress();
            
        baseLevelSlider.value = expGainedSlider.value = startProgress;
        
        currentLevelText.gameObject.SetActive(true);
        nextLevelText.gameObject.SetActive(true);
        baseLevelSlider.gameObject.SetActive(true);
        
        float elapsed = 0;
        float currentLevelAndProgress = startLevel + startProgress;
        while (elapsed <= 1) {
            yield return new WaitForEndOfFrame();
            elapsed += (Time.deltaTime / Mathf.Max(levelProgressTime, 0.0001f));
            
            currentLevelAndProgress = Mathf.Lerp(startLevel + startProgress, targetLevelAndProgress, elapsed);
            int currentLevel = Mathf.FloorToInt(currentLevelAndProgress);

            if (currentLevel > startLevel) baseLevelSlider.value = 0;
            expGainedSlider.value = currentLevelAndProgress % 1;
            
            currentLevelText.text = currentLevel.ToString("N0");
            nextLevelText.text = (currentLevel + 1).ToString("N0");
        }
        
        StartCoroutine(DoRewardsPanel());
    }
    
    IEnumerator DoRewardsPanel() {
        // Show rewards panel after delay.
        yield return new WaitForSeconds(macroDelay);
        levelPanelObject.SetActive(true);
        levelRewards.Toggle(true, false);
        
        for (int i = ManagerSaveLoad.GetLastRewardLevel(); i < ManagerSaveLoad.GetLevel(); i++) {
            yield return new WaitForSeconds(microDelay);
            levelRewards.CreateSingleRewardDisplay(i + 1);
        }
        
        yield return new WaitForSeconds(microDelay);
        nextLevelReward.SetInfo(ManagerLevelRewards.GetReward(ManagerSaveLoad.GetLevel(1)));
    }
}
