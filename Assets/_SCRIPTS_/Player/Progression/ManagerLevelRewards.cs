using System;
using System.Linq;
using Unity.Android.Gradle;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class ManagerLevelRewards : MonoSingleton<ManagerLevelRewards> {
    [Header("Item Rewards")]
    public DataBundleWeapons weaponRewards;
    public DataBundlePerks perkRewards;

    [Header("Blood Reward")]
    public DataLevelGenericReward bloodRewardDisplayable;
    public int bloodRewardAmount = 250;
    
    public static DataDisplayable GetReward(int level) => Inst.GetRewardAtLevel(level);
    public static DataDisplayable[] GetRewards(int startLevel, int endLevel) => Inst.GetRewardsInLevelRange(startLevel, endLevel);
    public static DataDisplayable[] GetUnrewardedRewards() => Inst.GetRewardsToBeRewarded();

    // ------ START METHODS ------

    protected override void OnAwake() => DontDestroyOnLoad(gameObject);
    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1) => TryGrantRewards();

    private void Start() {
        SceneManager.sceneLoaded += OnSceneLoaded;
        TryGrantRewards();
    }
    
    protected override void OnDestroyed(bool isDeletedInstance) { }

    // ------ EVENT METHODS ------
    
    public void TryGrantRewards() {
        if (!ManagerSaveLoad.CheckForRewards()) return;
        
        DataDisplayable[] rewards = GetRewardsToBeRewarded();
        foreach (DataDisplayable reward in rewards)
            if (reward is DataLevelGenericReward)
                ManagerSaveLoad.AddZombieBlood(bloodRewardAmount);
            
        ManagerSaveLoad.UpdateLastRewardLevel();
        ManagerSaveLoad.ForceSave();
    }
    
    // ------ HELPER METHODS ------

    private DataDisplayable GetRewardAtLevel(int level) => weaponRewards.HasWeaponAtLevel(level, out DataWeapon weapon) ? weapon : bloodRewardDisplayable;

    private DataDisplayable[] GetRewardsInLevelRange(int startLevel, int endLevel) {
        startLevel = Mathf.Max(startLevel, 1);
        if (endLevel < 1 || startLevel > endLevel) return Array.Empty<DataDisplayable>();
        
        DataDisplayable[] rewards = new DataDisplayable[(endLevel - startLevel) + 1];
        for (int i = 0; i < rewards.Length; i++)
            rewards[i] = GetRewardAtLevel(startLevel + i);

        return rewards;
    }

    private DataDisplayable[] GetRewardsToBeRewarded() => ManagerSaveLoad.CheckForRewards() ? GetRewardsInLevelRange(ManagerSaveLoad.GetLastRewardLevel() + 1, ManagerSaveLoad.GetLevel()) : null;
}