using System;
using System.IO;
using UnityEngine;

public class ManagerSaveLoad : MonoBehaviour {
    public static ManagerSaveLoad Instance;
    
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

    public static bool IsPlayerSaveLoaded => Instance.PlayerSaveLoaded;
    public static bool IsSettingsLoaded => Instance.SettingsLoaded;
    
    public static void ForceSave() => Instance.SaveData();
    public static void ForceLoad() => Instance.LoadData();
    public static void ClearPlayerSave() => Instance.ResetPlayerSave();
    public static void ClearPlayerSettings() => Instance.ResetSettings();
    public static void ClearAllData() { ClearPlayerSave(); ClearPlayerSettings(); }
    
    // ------ STATIC PROGRESSION METHODS ------
    
    public static int GetLevel() => Instance._playerSave.level;
    public static int GetExperience() => Instance._playerSave.experience;
    public static void AddExperience(int exp) {
        EventPlayerGainExp?.Invoke(exp * 15);
        Debug.Log("Player exp is boofin (fix for release)");
        if (Instance._playerSave.AddExperience(exp * 15) > 0)
            EventPlayerLevelUp?.Invoke(GetLevel());
    }
    
    public static int GetZombieBlood() => Instance._playerSave.zombieBlood;
    public static void AddZombieBlood(int amount) => Instance._playerSave.AddZombieBlood(amount);
    public static bool TrySpendZombieBlood(int cost) => Instance._playerSave.TrySpendZombieBlood(cost);

    public static int GetLootboxes() => Instance._playerSave.lootboxes;
    public static void AddLootboxes(int amount) => Instance._playerSave.AddLootboxes(amount);
    public static bool TryConsumeLootbox() => Instance._playerSave.TryConsumeLootbox();
    
    public static SaveLoadFilePerk[] GetPerkInstances(string perkName) => Instance._playerSave.GetPerkInstances();
    public static void AddPerkInstance(string perkName) => Instance._playerSave.AddPerkInstance(perkName);
    
    // ------ STATIC CAREER METHODS ------

    public static int GetCareerMaxWave() => Instance._playerSave.careerMaxWave;
    public static void SetCareerMaxWave(int wave) => Instance._playerSave.SetCareerMaxWave(wave);
    
    public static int GetCareerZombieBloodEarned() => Instance._playerSave.careerZombieBloodEarned;
    
    public static int GetCareerLootboxesOpened() => Instance._playerSave.careerLootboxesOpened;
    
    public static StringIntPair[] GetCareerZombieKillCounts() => Instance._playerSave.careerZombieKillCounts.ToArray();
    public static int GetCareerZombieKillCount(string zombieName) => Instance._playerSave.GetCareerZombieKillCount(zombieName);
    public static void AddCareerZombieKillCount(string zombieName, int kills = 1) => Instance._playerSave.AddCareerZombieKillCount(zombieName, kills);

    public static StringIntPair[] GetCareerDeathCauses() => Instance._playerSave.careerDeathCauses.ToArray();
    public static int GetCareerDeathCause(string deathSource) => Instance._playerSave.GetCareerDeathCause(deathSource);
    public static void AddCareerDeathCause(string deathSource, int deaths = 1) => Instance._playerSave.AddCareerDeathCause(deathSource, deaths);
    
    // ------ START METHODS ------
    
    void Awake() {
        if (Instance) {
            gameObject.SetActive(false);
            enabled = false;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
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
