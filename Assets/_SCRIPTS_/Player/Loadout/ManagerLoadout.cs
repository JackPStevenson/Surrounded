using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerLoadout : MonoSingleton<ManagerLoadout> {
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

    private bool loadOnNextScene;

    // ------ START METHODS ------

    protected override void OnAwake() {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    // ------ EVENT METHODS ------

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode) => ApplyLoadout();

    void ApplyLoadout() {
        if (!ManagerWeapon.Inst) return;

        foreach (DataWeaponTap w in _tapWeapons) if(w) ManagerWeapon.Inst.AddTap(w);
        foreach (DataWeaponSwipe w in _swipeWeapons) if(w) ManagerWeapon.Inst.AddSwipe(w);
        foreach (DataWeaponShake w in _shakeWeapons) if(w) ManagerWeapon.Inst.AddShake(w);
        foreach (DataPerkPlayer t in _playerPerks) if(t) PlayerCore.Inst.AddPerk(t);
    }
}