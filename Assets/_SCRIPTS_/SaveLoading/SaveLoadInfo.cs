using System;
using System.Collections.Generic;

public enum GraphicsSetting {
    Low,
    Medium,
    High
}

[Serializable]
public class SaveLoadSettings {
    public uint MusicVolume {get; set;}
    public uint EffectsVolume {get; set;}
    public GraphicsSetting Graphics {get; set;}
    public bool Vibration {get; set;}
    
    public SaveLoadSettings(uint music = 1, uint effects = 1, GraphicsSetting graphics = GraphicsSetting.High, bool vibration = true) {
        MusicVolume = music;
        EffectsVolume = effects;
        Graphics = graphics;
        Vibration = vibration;
    }
}

[Serializable]
public class SaveLoadPlayer {
    public uint Level { get; set; }
    public uint Experience { get; set; }
    public uint ZombieBlood { get; set; }

    public uint MaxWave { get; set; }
    public uint TotalZombieBlood { get; set; }
    public List<StringUIntPair> ZombieKills { get; set; }
    public List<StringUIntPair> DeathsToZombies { get; set; }

    public List<string> TapWeaponUnlocks { get; set; }
    public List<string> SwipeWeaponUnlocks { get; set; }
    public List<string> ShakeWeaponUnlocks { get; set; }

    public List<SaveLoadPerkInstance> PerkInstances { get; set; }
    
    public SaveLoadPlayer(uint level = 1, uint exp = 0, uint blood = 0, uint maxWave = 0, uint totalBlood = 0) {
        Level = level;
        Experience = exp;
        ZombieBlood = blood;
        
        MaxWave = maxWave;
        TotalZombieBlood = totalBlood;
        ZombieKills = new List<StringUIntPair>();
        DeathsToZombies = new List<StringUIntPair>();
        
        TapWeaponUnlocks = new List<string>();
        SwipeWeaponUnlocks = new List<string>();
        ShakeWeaponUnlocks = new List<string>();
        
        PerkInstances = new List<SaveLoadPerkInstance>();
    }
}

[Serializable]
public class SaveLoadPerkInstance {
    public uint PerkId {get; set;}
    //public SaveLoadPerkVariance[] perkVariance;
}