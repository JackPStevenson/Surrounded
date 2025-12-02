using System;
using System.Linq;
using Unity.Android.Gradle;
using UnityEngine;
using Random = UnityEngine.Random;

public class ManagerLevelRewards : MonoSingleton<ManagerLevelRewards> {
    public int rewardSeed = 0;
    public DataBundleWeapons weaponRewards;
    public DataBundlePerks perkRewards;

    public DataLevelGenericReward[] genericRewards;

    public int bloodPerBloodReward;
    public int lootboxesPerLootboxReward;

    // ------ START METHODS ------

    protected override void OnAwake() {

    }

    // ------ HELPER METHODS ------

    public DataDisplayable GetRewardAtLevel(int level) {
        if (weaponRewards.weapons.FirstOrDefault(w => w.levelToUnlock == level) is DataDisplayable reward) return reward;
        
        Random.InitState(level + rewardSeed);
        return genericRewards[Random.Range(0, genericRewards.Length)];
    }

    public DataDisplayable[] GetRewardsInLevelRange(int startLevel, int endLevel) {
        startLevel = Mathf.Max(startLevel, 1);
        if (endLevel < 1 || startLevel > endLevel) return Array.Empty<DataDisplayable>();
        
        DataDisplayable[] rewards = new DataDisplayable[(endLevel - startLevel) + 1];
        for (int i = 0; i < rewards.Length; i++)
            rewards[i] = GetRewardAtLevel(startLevel + i);

        return rewards;
    }

    public DataDisplayable[] GetRewardsToBeRewarded() => ManagerSaveLoad.CheckRewardsForPlayer() ? GetRewardsInLevelRange(ManagerSaveLoad.GetLastRewardedLevel() + 1, ManagerSaveLoad.GetLevel()) : null;
}