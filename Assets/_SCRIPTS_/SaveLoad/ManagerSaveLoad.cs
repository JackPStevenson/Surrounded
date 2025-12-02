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
    public static void ClearAllData() { ClearPlayerSave(); ClearPlayerSettings(); }
    
    // ------ STATIC PROGRESSION METHODS ------
    
    public static int GetLevel() => Inst._playerSave.level;
    public static bool CheckLevel(int level) => GetLevel() >= level;
    public static int GetExperience() => Inst._playerSave.experience;
    public static float GetLevelProgress() => (float) Inst._playerSave.experience / SaveLoadPlayer.ExperiencePerLevel;
    public static void AddExperience(int exp) {
        EventPlayerGainExp?.Invoke(exp * 15);
        Debug.Log("Player exp is boofin (fix for release)");
        if (Inst._playerSave.AddExperience(exp * 15) > 0)
            EventPlayerLevelUp?.Invoke(GetLevel());
    }
    
    public static int GetLastRewardedLevel() => Inst._playerSave.lastRewardedLevel;
    public static bool CheckRewardsForPlayer(bool updateLastRewardedLevel = false) => Inst._playerSave.CheckRewardsForPlayer(updateLastRewardedLevel);
    public static int GetZombieBlood() => Inst._playerSave.zombieBlood;
    public static void AddZombieBlood(int amount) => Inst._playerSave.AddZombieBlood(amount);
    public static bool TrySpendZombieBlood(int cost) => Inst._playerSave.TrySpendZombieBlood(cost);

    public static int GetLootboxes() => Inst._playerSave.lootboxes;
    public static void AddLootboxes(int amount) => Inst._playerSave.AddLootboxes(amount);
    public static bool TryConsumeLootbox() => Inst._playerSave.TryConsumeLootbox();
    
    public static SaveLoadFilePerk[] GetPerkInstances(string perkName) => Inst._playerSave.GetPerkInstances();
    public static void AddPerkInstance(string perkName) => Inst._playerSave.AddPerkInstance(perkName);
    
    // ------ STATIC CAREER METHODS ------

    public static int GetCareerMaxWave() => Inst._playerSave.careerMaxWave;
    public static void SetCareerMaxWave(int wave) => Inst._playerSave.SetCareerMaxWave(wave);
    
    public static int GetCareerZombieBloodEarned() => Inst._playerSave.careerZombieBloodEarned;
    
    public static int GetCareerLootboxesOpened() => Inst._playerSave.careerLootboxesOpened;
    
    public static StringIntPair[] GetCareerZombieKillCounts() => Inst._playerSave.careerZombieKillCounts.ToArray();
    public static int GetCareerZombieKillCount(string zombieName) => Inst._playerSave.GetCareerZombieKillCount(zombieName);
    public static void AddCareerZombieKillCount(string zombieName, int kills = 1) => Inst._playerSave.AddCareerZombieKillCount(zombieName, kills);

    public static StringIntPair[] GetCareerDeathCauses() => Inst._playerSave.careerDeathCauses.ToArray();
    public static int GetCareerDeathCause(string deathSource) => Inst._playerSave.GetCareerDeathCause(deathSource);
    public static void AddCareerDeathCause(string deathSource, int deaths = 1) => Inst._playerSave.AddCareerDeathCause(deathSource, deaths);
    
    // ------ START METHODS ------
    
    protected override void OnAwake() {
        DontDestroyOnLoad(gameObject);
        LoadData();
    }

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
