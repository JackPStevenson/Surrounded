using System;
using System.IO;
using UnityEngine;

public class ManagerSaveLoad : MonoSingleton<ManagerSaveLoad> {
    public static event Action<int> EventPlayerGainExp;
    public static event Action<int> EventPlayerLevelUp;

    private const string PlayerSaveName = "PlayerSave";
    private const string PlayerSettingsName = "PlayerSettings";
    private static string DataDir => Application.persistentDataPath + "/";

    public string PlayerSavePath => DataDir + PlayerSaveName + ".json";
    public string PlayerSettingsPath => DataDir + PlayerSettingsName + ".json";

    [SerializeField]
    private SaveLoadPlayer _playerSave;
    [Space]
    [SerializeField]
    private SaveLoadSettings _playerSettings;

    private bool PlayerSaveLoaded => _playerSave != null;
    private bool SettingsLoaded => _playerSettings != null;

    // ------ STATIC SAVE LOAD METHODS ------

    public static bool IsPlayerSaveLoaded => Inst.PlayerSaveLoaded;
    public static bool IsSettingsLoaded => Inst.SettingsLoaded;

    public static void ForceSave() => Inst.SaveData();
    public static void ForceLoad() => Inst.LoadData();
    public static void ClearPlayerSave() => Inst.ResetPlayerSave();
    public static void ClearPlayerSettings() => Inst.ResetSettings();
    public static void ClearAllData() {
        ClearPlayerSave();
        ClearPlayerSettings();
    }

    // ------ STATIC PROGRESSION METHODS ------

    public static int GetLevel(int offset = 0) => Inst._playerSave.level + offset;
    public static bool CheckLevel(int level) => level < 1 || GetLevel() >= level;
    public static int GetExperience() => Inst._playerSave.experience;
    public static int GetExperiencePerLevel() => SaveLoadPlayer.ExperiencePerLevel;
    public static float GetLevelProgress() => (float) Inst._playerSave.experience / SaveLoadPlayer.ExperiencePerLevel;
    public static void AddExperience(int exp) {
        EventPlayerGainExp?.Invoke(exp);
        if (Inst._playerSave.AddExperience(exp) > 0)
            EventPlayerLevelUp?.Invoke(GetLevel());
    }

    public static int GetLastRewardLevel() => Inst._playerSave.lastRewardLevel;
    public static int GetPendingRewardCount() => Inst._playerSave.GetPendingRewardCount();
    public static void UpdateLastRewardLevel() => Inst._playerSave.UpdateLastRewardLevel();
    public static bool CheckForRewards() => Inst._playerSave.HasPendingRewards();
    public static int GetZombieBlood() => Inst._playerSave.zombieBlood;
    public static void AddZombieBlood(int amount) => Inst._playerSave.AddZombieBlood(amount);
    public static bool TrySpendZombieBlood(int cost) => Inst._playerSave.TrySpendZombieBlood(cost);

    public static SaveLoadFilePerk[] GetPerkInstances() => Inst._playerSave.GetPerkInstances();
    public static void AddPerkInstance(string perkName, float perkPower) => Inst._playerSave.AddPerkInstance(perkName, perkPower);

    // ------ STATIC CAREER METHODS ------

    public static int GetCareerMaxWave() => Inst._playerSave.careerMaxWave;
    public static void TrySetCareerMaxWave(int wave) => Inst._playerSave.SetCareerMaxWave(Mathf.Max(GetCareerMaxWave(), wave));

    public static int GetCareerZombieBloodEarned() => Inst._playerSave.careerZombieBloodEarned;

    public static StringIntPair[] GetCareerZombieKillCounts() => Inst._playerSave.careerZombieKillCounts.ToArray();
    public static int GetCareerZombieKillCount(string zombieName) => Inst._playerSave.GetCareerZombieKillCount(zombieName);
    public static void AddCareerZombieKillCount(string zombieName, int kills = 1) => Inst._playerSave.AddCareerZombieKillCount(zombieName, kills);

    public static StringIntPair[] GetCareerDeathCauses() => Inst._playerSave.careerDeathCauses.ToArray();
    public static int GetCareerDeathCause(string deathSource) => Inst._playerSave.GetCareerDeathCause(deathSource);
    public static void AddCareerDeathCause(string deathSource, int deaths = 1) => Inst._playerSave.AddCareerDeathCause(deathSource, deaths);

    // ------ STATIC SETTINGS METHODS ------
    
    public static float GetEffectsVolume() => Inst._playerSettings.effectsVolume;
    public static void SetEffectsVolume(float volume) => Inst._playerSettings.SetEffectsVolume(volume);
    
    public static float GetMusicVolume() => Inst._playerSettings.musicVolume;
    public static void SetMusicVolume(float volume) => Inst._playerSettings.SetMusicVolume(volume);
    
    // ------ START METHODS ------

    protected override void OnAwake() {
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

    protected override void OnDestroyed(bool isDeletedInstance) { if (isDeletedInstance) SaveData(); }

// ------ SAVE LOAD METHODS ------
    
    private void SaveData() {
        if (!SaveLoadUtils.TrySave(PlayerSavePath, _playerSave)) Debug.LogError("Failed to write Player Save to disc.");
        if (!SaveLoadUtils.TrySave(PlayerSettingsPath, _playerSettings)) Debug.LogError("Failed to write Player Settings to disc.");
    }
    
    private void LoadData() {
        LoadPlayerSave();
        LoadSettings();
    }
    
    private void LoadPlayerSave() {
        if (SaveLoadUtils.TryLoad(PlayerSavePath, out SaveLoadPlayer player)) _playerSave = player;
        else {
            Debug.Log("Making new player save...");
            _playerSave ??= new SaveLoadPlayer();
            SaveData();
        }
    }

    private void LoadSettings() {
        if (SaveLoadUtils.TryLoad(PlayerSettingsPath, out SaveLoadSettings settings)) _playerSettings = settings;
        else {
            Debug.Log("Making new player settings...");
            _playerSettings ??= new SaveLoadSettings();
            SaveData();
        }
    }
    
    // ------ RESET METHODS ------
    
    private void ResetPlayerSave() {
        if (!PlayerSaveLoaded) return;
        
        try { File.Delete(PlayerSavePath); }
        catch (Exception e) { /* ignore */ }
        
        _playerSave = null;
        LoadPlayerSave();
    }
    
    private void ResetSettings() {
        if (!SettingsLoaded) return;
        
        try { File.Delete(PlayerSettingsPath); }
        catch (Exception e) { /* ignore */ }
        
        _playerSettings = null;
        LoadSettings();
    }
}
