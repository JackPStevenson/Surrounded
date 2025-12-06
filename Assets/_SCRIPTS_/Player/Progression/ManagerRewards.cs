using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerRewards : MonoSingleton<ManagerRewards> {
    [Header("References")]
    public DataBundleMaster masterBundle;
    public DataLevelGenericReward bloodRewardDisplayable;

    [Header("Waves")]
    public int bloodPerWaveBonus = 25;
    
    [Header("Experience")]
    public int bloodToExpMultiplier = 2;
    public int bloodToExpBonus = 10;
    
    [Header("Level Rewards")]
    public int bloodLevelRewardAmount = 250;

    [Header("Perks")]
    public AnimationCurve perkQualityCurve;

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
                ManagerSaveLoad.AddZombieBlood(BloodLevelRewardAmount);
            
        ManagerSaveLoad.UpdateLastRewardLevel();
        ManagerSaveLoad.ForceSave();
    }

    // ------ BLOOD REWARD METHODS ------
    
    public static int GetBloodFromWaves (int waves) => waves * Inst.bloodPerWaveBonus;
    public static int GetExpFromWaves (int waves) => GetExp(GetBloodFromWaves(waves), waves);
    public static int GetBloodFromZombies(string name, int count) {
        Debug.Log(Inst.masterBundle.Zombies.GetZombie(name));
        return (Inst.masterBundle.Zombies.GetZombie(name) is { } zombie) ? zombie.bloodOnDeath * count : 0;
    }
    public static int GetExpFromZombies (string name, int count) => GetExp(GetBloodFromZombies(name, count), count);
    
    public static int GetExp(int totalBlood, int bonusCount) => (totalBlood * Inst.bloodToExpMultiplier) + (Inst.bloodToExpBonus * bonusCount);
    public static int BloodLevelRewardAmount => Inst.bloodLevelRewardAmount;


    public static void GrantPerformanceRewards(PlayerStats playerStats) {
        // Give player blood and exp based on performance.
        ManagerSaveLoad.AddZombieBlood(playerStats.TotalBlood);
        ManagerSaveLoad.AddExperience(playerStats.TotalExp);

        // Update player's career stats.
        ManagerSaveLoad.TrySetCareerMaxWave(playerStats.Wave);
        if (playerStats.KilledBy.Length > 0) ManagerSaveLoad.AddCareerDeathCause(playerStats.KilledBy);
        foreach (PlayerStat stat in playerStats.SpecificZombieKills)
            ManagerSaveLoad.AddCareerZombieKillCount(stat.Name, stat.Amount);
        
        // Force save once all rewards are given.
        ManagerSaveLoad.ForceSave();
    }
    
    // ------ LEVEL REWARD METHODS ------
    
    public static DataDisplayable GetLevelReward(int level) => Inst.GetRewardAtLevel(level);
    public static DataDisplayable[] GetLevelRewards(int startLevel, int endLevel) => Inst.GetRewardsInLevelRange(startLevel, endLevel);
    public static DataDisplayable[] GetLevelRewardsToBeRewarded() => Inst.GetRewardsToBeRewarded();
    

    private DataDisplayable GetRewardAtLevel(int level) => masterBundle.Weapons.HasWeaponAtLevel(level, out DataWeapon weapon) ? weapon : bloodRewardDisplayable;

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