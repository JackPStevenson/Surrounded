using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveLoadPerkInstance {
    public int perkId;
    //public SaveLoadPerkVariance[] perkVariance;
}

[Serializable]
public class SaveLoadPlayer {
    public const int ExperiencePerLevel = 1000;
    
    // --- PROGRESSION ---
    public int level;
    public int experience;
    public int zombieBlood;

    public List<string> tapWeaponUnlocks;
    public List<string> swipeWeaponUnlocks;
    public List<string> shakeWeaponUnlocks;
    public List<SaveLoadPerkInstance> perkUnlocks;

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
        
        tapWeaponUnlocks = new List<string>();
        swipeWeaponUnlocks = new List<string>();
        shakeWeaponUnlocks = new List<string>();
        perkUnlocks = new List<SaveLoadPerkInstance>();
        
        this.careerMaxWave = careerMaxWave;
        this.careerZombieBloodEarned = careerZombieBloodEarned;
        careerZombieKills = new List<StringIntPair>();
        careerDeathsToZombies = new List<StringIntPair>();
    }

    // ------ EVENT METHODS ------
    
    public int AddExperience(int exp) {
        // Add to current experience and calculate how many levels were gained from added experience.
        experience += exp;
        int levelsGained = Mathf.FloorToInt(experience / (float) ExperiencePerLevel);
        experience %= ExperiencePerLevel;

        // Add gained levels to current level and return how many levels were gained.
        level += levelsGained;
        return levelsGained;
    }
}

public enum GraphicsSetting {
    Low,
    Medium,
    High
}

[Serializable]
public class SaveLoadSettings {
    public int musicVolume;
    public int effectsVolume;
    public GraphicsSetting graphics;
    public bool vibration;
    
    // ------ CONSTRUCTORS ------
    
    public SaveLoadSettings(int music = 1, int effects = 1, GraphicsSetting graphics = GraphicsSetting.High, bool vibration = true) {
        musicVolume = music;
        effectsVolume = effects;
        this.graphics = graphics;
        this.vibration = vibration;
    }
}