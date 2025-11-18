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

    public List<string> weaponUnlocks;
    public List<SaveLoadFilePerk> perkInstances;

    // --- CAREER ---
    public int careerMaxWave;
    public int careerZombieBloodEarned;
    public List<StringIntPair> careerZombieKills;
    public List<StringIntPair> careerDeathsToZombies;
    
    // ------ CONSTRUCTORS ------
    
    public SaveLoadPlayer(int level = 1, int experience = 0, int zombieBlood = 0, int careerMaxWave = 0, int careerZombieBloodEarned = 0) {
        this.level = level;
        this.experience = experience;
        this.zombieBlood = zombieBlood;
        
        weaponUnlocks = new List<string>();
        perkInstances = new List<SaveLoadFilePerk>();
        
        this.careerMaxWave = careerMaxWave;
        this.careerZombieBloodEarned = careerZombieBloodEarned;
        careerZombieKills = new List<StringIntPair>();
        careerDeathsToZombies = new List<StringIntPair>();
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

    public void UnlockWeapon(string weaponName) { if(!HasWeaponUnlocked(weaponName)) weaponUnlocks.Add(weaponName); }
    public bool HasWeaponUnlocked(string weaponName) => weaponUnlocks.Any(t => string.CompareOrdinal(t, weaponName) == 0);

    public void AddPerkInstance(string perkName) => perkInstances.Add(new SaveLoadFilePerk(perkName));
    public SaveLoadFilePerk[] GetPerkInstances() => perkInstances.ToArray();

    // ------ CAREER METHODS ------

    public void AddZombieKills(string zombieName, int kills) => StringIntPair.TryAddToList(careerZombieKills, zombieName, kills);
    public void AddDeathsToZombie(string zombieName, int deaths = 1) => StringIntPair.TryAddToList(careerDeathsToZombies, zombieName, deaths);
}