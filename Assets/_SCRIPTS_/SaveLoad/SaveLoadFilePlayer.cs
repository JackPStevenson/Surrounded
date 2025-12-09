using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[Serializable]
public class SaveLoadPlayer {
    public const int ExperiencePerLevel = 1000;
    
    // --- PROGRESSION ---
    public int level;
    public int experience;

    public int lastRewardLevel;
    public int zombieBlood;

    public List<SaveLoadFilePerk> perkInstances;

    // --- CAREER ---
    public int careerMaxWave;
    public int careerZombieBloodEarned;
    public List<StringIntPair> careerZombieKillCounts;
    public List<StringIntPair> careerDeathCauses;
    
    // ------ CONSTRUCTORS ------
    
    public SaveLoadPlayer(int level = 0, int experience = 0, int lastRewardLevel = 0, int zombieBlood = 0, int careerMaxWave = 0, int careerZombieBloodEarned = 0) {
        this.level = level;
        this.experience = experience;
        
        this.lastRewardLevel = lastRewardLevel;
        this.zombieBlood = zombieBlood;
        
        perkInstances = new List<SaveLoadFilePerk>();
        
        this.careerMaxWave = careerMaxWave;
        this.careerZombieBloodEarned = careerZombieBloodEarned;
        careerZombieKillCounts = new List<StringIntPair>();
        careerDeathCauses = new List<StringIntPair>();
    }

    // ------ PROGRESSION METHODS ------
    
    public int AddExperience(int exp) {
        // Add to current experience and calculate how many levels were gained from added experience.
        experience += exp;
        int levelsGained = Mathf.FloorToInt(experience / (float) ExperiencePerLevel);
        experience %= ExperiencePerLevel;

        // Add gained levels to current level and return how many levels were gained.
        level += levelsGained;
        return levelsGained;
    }
    
    public void UpdateLastRewardLevel() => lastRewardLevel = level;
    public bool HasPendingRewards() => GetPendingRewardCount() > 0;
    public int GetPendingRewardCount() => Mathf.Max(level - lastRewardLevel, 0);
    
    public int[] GetRewards() {
        if (lastRewardLevel >= level) return Array.Empty<int>();

        // Add each level that needs to be rewarded to a list.
        int[] levelsToReward = new int[level - lastRewardLevel];
        for (int i = 1; i <= levelsToReward.Length; i++)
            levelsToReward[i] = lastRewardLevel + i;
        
        // Return list of each level to be rewarded.
        return levelsToReward;
    }
    
    public void AddZombieBlood(int amount) {
        zombieBlood += Mathf.Max(amount, 0);
        careerZombieBloodEarned += Mathf.Max(amount, 0);
    }
    public bool TrySpendZombieBlood(int cost) {
        if (zombieBlood < cost) return false;
        zombieBlood -= Mathf.Max(cost, 0);
        return true;
    }
    
    public void AddPerkInstance(int perkIndex, float perkPower) => perkInstances.Add(new SaveLoadFilePerk(perkIndex, perkPower));
    public SaveLoadFilePerk[] GetPerkInstances() => perkInstances.ToArray();

    // ------ CAREER METHODS ------

    public int SetCareerMaxWave(int wave) => careerMaxWave = wave;
    
    public int GetCareerZombieKillCount(string zombieName) => StringIntPair.TryGetFromList(careerZombieKillCounts, zombieName, 0);
    public void AddCareerZombieKillCount(string zombieName, int kills = 1) => StringIntPair.TryAddToList(careerZombieKillCounts, zombieName, kills);
    
    public int GetCareerDeathCause(string deathSource) => StringIntPair.TryGetFromList(careerDeathCauses, deathSource, 0);
    public void AddCareerDeathCause(string deathSource, int deaths = 1) => StringIntPair.TryAddToList(careerDeathCauses, deathSource, deaths);
}