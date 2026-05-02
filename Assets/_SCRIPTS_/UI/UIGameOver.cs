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
    public UIDisplayItem killedByDisplay;
    public UIPerformanceStatDisplay wavesDisplay;
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

    public void StartDisplay() => StartCoroutine(DoPerformancePanel());

    IEnumerator DoPerformancePanel() {
        PlayerStats stats = PlayerStatTracker.PlayerStats;
        UIPerformanceStatDisplay temp;
        
        // Show performance panel after delay.
        yield return new WaitForSeconds(macroDelay);
        performancePanelObject.SetActive(true);
        
        // Show killed by stat after delay.
        yield return new WaitForSeconds(microDelay);
        killedByDisplay.SetInfo("Eaten By", stats.KilledBy);
        
        // Show wave stat after delay.
        yield return new WaitForSeconds(microDelay);
        wavesDisplay.SetInfo(stats.WaveReached);
        
        // Show total zombies stat after delay. After, display each zombie sub-stat.
        yield return new WaitForSeconds(microDelay);
        totalZombiesDisplay.SetInfo(stats.TotalZombieKills);
        foreach (PlayerStat zombieKillStat in stats.SpecificZombieKills) {
            yield return new WaitForSeconds(microDelay);
            Instantiate(performanceStatPrefab, zombiesSubDisplayHolder).TryGetComponent(out temp);
            temp.SetInfo(zombieKillStat);
            
            string editedName = temp.GetName();
            editedName = editedName.Replace(" Zombie", "");
            temp.SetName(editedName);
        }
        
        // Show total earnings. After, start level panel display.
        yield return new WaitForSeconds(microDelay);
        totalEarningsDisplay.SetInfo(stats.TotalEarnings);
        StartCoroutine(DoLevelPanel());
    }

    IEnumerator DoLevelPanel() {
        PlayerStats stats = PlayerStatTracker.PlayerStats;
        
        // Show level panel after delay.
        yield return new WaitForSeconds(macroDelay);
        levelPanelObject.SetActive(true);
        
        yield return new WaitForSeconds(microDelay);
        currentLevelText.text = stats.StartLevel.ToString("N0");
        nextLevelText.text = (stats.StartExp + 1).ToString("N0");

        float startProgress = ((float) stats.StartExp / ManagerSaveLoad.GetExperiencePerLevel());
        float targetLevelAndProgress = ManagerSaveLoad.GetLevel() + ManagerSaveLoad.GetLevelProgress();
            
        baseLevelSlider.value = expGainedSlider.value = startProgress;
        
        currentLevelText.gameObject.SetActive(true);
        nextLevelText.gameObject.SetActive(true);
        baseLevelSlider.gameObject.SetActive(true);
        
        float elapsed = 0;
        float currentLevelAndProgress = stats.StartLevel + startProgress;
        while (elapsed <= 1) {
            yield return new WaitForEndOfFrame();
            elapsed += (Time.deltaTime / Mathf.Max(levelProgressTime, 0.0001f));
            
            currentLevelAndProgress = Mathf.Lerp(stats.StartLevel + startProgress, targetLevelAndProgress, elapsed);
            int currentLevel = Mathf.FloorToInt(currentLevelAndProgress);

            if (currentLevel > stats.StartLevel) baseLevelSlider.value = 0;
            expGainedSlider.value = currentLevelAndProgress % 1;
            
            currentLevelText.text = currentLevel.ToString("N0");
            nextLevelText.text = (currentLevel + 1).ToString("N0");
        }
        
        StartCoroutine(DoRewardsPanel());
    }
    
    IEnumerator DoRewardsPanel() {
        PlayerStats stats = PlayerStatTracker.PlayerStats;
        
        // Show rewards panel after delay.
        yield return new WaitForSeconds(macroDelay);
        levelPanelObject.SetActive(true);
        levelRewards.Toggle(true, false);
        
        for (int i = ManagerSaveLoad.GetLastRewardLevel(); i < ManagerSaveLoad.GetLevel(); i++) {
            yield return new WaitForSeconds(microDelay);
            levelRewards.CreateSingleRewardDisplay(i + 1);
        }
        
        yield return new WaitForSeconds(microDelay);
        DataDisplayable nextReward = ManagerRewards.GetLevelReward(ManagerSaveLoad.GetLevel(1));
        
        nextLevelReward.SetInfo(nextReward, ManagerSaveLoad.GetLevel(1));
        if(nextReward is DataWeapon) nextLevelReward.Button.onClick.AddListener(() => levelRewards.itemPreview.PreviewItem(nextReward));
    }
}
