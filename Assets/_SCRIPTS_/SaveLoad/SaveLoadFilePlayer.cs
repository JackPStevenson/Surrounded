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
    
    public int zombieBlood;
    public int lootboxes;

    public List<SaveLoadFilePerk> perkInstances;

    // --- CAREER ---
    public int careerMaxWave;
    public int careerZombieBloodEarned;
    public int careerLootboxesOpened;
    public List<StringIntPair> careerZombieKillCounts;
    public List<StringIntPair> careerDeathCauses;
    
    // ------ CONSTRUCTORS ------
    
    public SaveLoadPlayer(int level = 1, int experience = 0, int zombieBlood = 0, int lootboxes = 0, int careerMaxWave = 0, int careerZombieBloodEarned = 0, int careerLootboxesOpened = 0) {
        this.level = level;
        this.experience = experience;
        this.zombieBlood = zombieBlood;
        
        perkInstances = new List<SaveLoadFilePerk>();
        
        this.careerMaxWave = careerMaxWave;
        this.careerZombieBloodEarned = careerZombieBloodEarned;
        this.careerLootboxesOpened = careerLootboxesOpened;
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
        Debug.Log("Level: " + level + "  Experience: " + experience);
        return levelsGained;
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

    public void AddLootboxes(int amount) => lootboxes += amount;
    public bool TryConsumeLootbox() {
        if (lootboxes <= 0) return false;
        lootboxes--;
        careerLootboxesOpened++;
        return true;
    }

    public void AddPerkInstance(string perkName) => perkInstances.Add(new SaveLoadFilePerk(perkName));
    public SaveLoadFilePerk[] GetPerkInstances() => perkInstances.ToArray();

    // ------ CAREER METHODS ------

    public int SetCareerMaxWave(int wave) => careerMaxWave = wave;
    
    public int GetCareerZombieKillCount(string zombieName) => StringIntPair.TryGetFromList(careerZombieKillCounts, zombieName, 0);
    public void AddCareerZombieKillCount(string zombieName, int kills = 1) => StringIntPair.TryAddToList(careerZombieKillCounts, zombieName, kills);
    
    public int GetCareerDeathCause(string deathSource) => StringIntPair.TryGetFromList(careerDeathCauses, deathSource, 0);
    public void AddCareerDeathCause(string deathSource, int deaths = 1) => StringIntPair.TryAddToList(careerDeathCauses, deathSource, deaths);
}