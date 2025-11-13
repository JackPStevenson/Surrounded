using System;
using System.IO;
using UnityEngine;

public class ManagerSaveLoad : MonoBehaviour {
    public static ManagerSaveLoad Instance;
    
    private const string PlayerSaveName = "PlayerSave";
    private const string PlayerSettingsName = "PlayerSettings";
    private static string DataDir => Application.persistentDataPath + "/";

    public string PlayerSavePath => DataDir + PlayerSaveName + ".json";
    public string PlayerSettingsPath => DataDir + PlayerSettingsName + ".json";


    private SaveLoadPlayer _playerSave;
    private SaveLoadSettings _playerSettings;
    private bool PlayerSaveLoaded => _playerSave != null;
    private bool SettingsLoaded => _playerSettings != null;
    
    // ------ STATIC METHODS ------

    public static void ForceSave() => Instance.SaveData();
    public static void ForceLoad() => Instance.LoadData();
    
    public static void ClearPlayerSave() => Instance.ResetPlayerSave();
    public static void ClearPlayerSettings() => Instance.ResetSettings();
    public static void ClearAllData() { ClearPlayerSave(); ClearPlayerSettings(); }
    
    public static bool IsPlayerSaveLoaded => Instance.PlayerSaveLoaded;
    public static bool IsSettingsLoaded => Instance.SettingsLoaded;


    // ------ START METHODS ------
    
    void Awake() {
        if (Instance) {
            gameObject.SetActive(false);
            enabled = false;
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        ClearAllData();
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
        Debug.Log("Wiping Player Save");
        if (!PlayerSaveLoaded) return;
        try {
            File.Delete(PlayerSavePath);
        }
        catch (Exception e) { /* ignore */ }
        LoadPlayerSave();
    }
    
    private void ResetSettings() {
        Debug.Log("Wiping Settings");
        if (!SettingsLoaded) return;
        try { File.Delete(PlayerSettingsPath); }
        catch (Exception e) { /* ignore */ }
        LoadSettings();
    }
}
