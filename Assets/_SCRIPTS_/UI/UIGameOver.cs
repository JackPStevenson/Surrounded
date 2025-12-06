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
    public Slider baseLevelSlider;
    public Slider expGainedSlider;
    public UIDisplayItemLevelReward nextLevelReward;

    [Header("Rewards Panel")]
    public GameObject rewardsPanelObject;
    [Space]
    public UILevelRewards levelRewards;

    public void StartDisplay(UIStat wave, UIStat[] generic, UIStat totalZombieKills, UIStat[] zombieKills, UIStat totalEarnings, int startLevel, int newLevel) {
        
        StartCoroutine(DoPerformancePanel(wave, generic, totalZombieKills, zombieKills, totalEarnings, startLevel, newLevel));
    }

    IEnumerator DoPerformancePanel(UIStat wave, UIStat[] generic, UIStat totalZombieKills, UIStat[] zombieKills, UIStat totalEarnings, int startLevel, int newLevel) {
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
        StartCoroutine(DoLevelPanel(startLevel, newLevel));
    }

    IEnumerator DoLevelPanel(int startLevel, int newLevel) {
        // Show level panel after delay.
        yield return new WaitForSeconds(macroDelay);
        levelPanelObject.SetActive(true);
        
        StartCoroutine(DoRewardsPanel(newLevel));
    }
    
    IEnumerator DoRewardsPanel(int newLevel) {
        // Show rewards panel after delay.
        yield return new WaitForSeconds(macroDelay);
        levelPanelObject.SetActive(true);
    }
}
