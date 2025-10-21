using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerLoadout : MonoBehaviour {
    public static ManagerLoadout Instance;

    // --- TAP WEAPONS ---
    private readonly List<DataWeaponTap> _tapWeapons = new List<DataWeaponTap>();
    public void AddTap(DataWeaponTap data) => _tapWeapons.Add(data);
    public void ClearTaps() => _tapWeapons.Clear();
    
    // --- SWIPE WEAPONS ---
    private readonly List<DataWeaponSwipe> _swipeWeapons = new List<DataWeaponSwipe>();
    public void AddSwipe(DataWeaponSwipe data) {
        _swipeWeapons.Add(data);
    }
    public void ClearSwipes() => _swipeWeapons.Clear();
    
    // --- SHAKE WEAPONS ---
    private readonly List<DataWeaponShake> _shakeWeapons = new List<DataWeaponShake>();
    public void AddShake(DataWeaponShake data) => _shakeWeapons.Add(data);
    public void ClearShakes() => _shakeWeapons.Clear();

    // --- PERKS ---
    private readonly DataPerkPlayer[] _playerPerks = new DataPerkPlayer[3];
    public void SetPerkPlayer(DataPerkPlayer data, int index) => _playerPerks[index] = data;

    private bool configured = false;

    // ------ START METHODS ------

    void Awake() {
        Instance = this;
    }

    void Start() {
        if (configured) return;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
        configured = true;
    }
    
    // ------ EVENT METHODS ------

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) => ApplyLoadout();

    void ApplyLoadout() {
        ManagerWeapon managerWeapon = ManagerWeapon.Instance;
        PoorSoulCore poorSoul = PoorSoulCore.Instance;
        if (!managerWeapon || !poorSoul) return;
        

        foreach (DataWeaponTap w in _tapWeapons) if(w) managerWeapon.AddTap(w);
        foreach (DataWeaponSwipe w in _swipeWeapons) if(w) managerWeapon.AddSwipe(w);
        foreach (DataWeaponShake w in _shakeWeapons) if(w) managerWeapon.AddShake(w);
        foreach (DataPerkPlayer t in _playerPerks) if(t) poorSoul.AddPerk(t);
        
        Destroy(gameObject);
    }
}