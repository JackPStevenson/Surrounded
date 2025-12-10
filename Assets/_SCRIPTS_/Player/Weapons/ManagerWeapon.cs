using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWeapon : MonoSingleton<ManagerWeapon> {
    public event Action<Health[]> EventOnHit;
    public event Action<Health[]> EventOnKill;
    public event Action<Vector3> EventOnTap;
    public event Action<Vector3[]> EventOnSwipe;
    public event Action EventOnShake;
    public event Action<DataWeapon, int> EventWeaponEquip;
    private ManagerInput _managerInput;

    [Header("Tapping")]
    public float floorHeight = 0;
    public float tapOffsetTowardsCamera = 0;
    public float tapHeightOffset = 0.5f;

    [Header("Weapons")]
    public WeaponTap fallbackTap;
    protected int ActiveTap = -1;
    private bool _useFallback = false;
    public List<WeaponTap> tapWeapons = new List<WeaponTap>();
    public WeaponTap CurrentTap => ActiveTap > -1 && tapWeapons != null && ActiveTap < tapWeapons.Count ? tapWeapons[ActiveTap] : null;
    public float CurrentTapEnergy => CurrentTap ? CurrentTap.CurrentEnergy : 0;
    [Space]
    protected int ActiveSwipe = -1;
    public List<WeaponSwipe> swipeWeapons = new List<WeaponSwipe>();
    public WeaponSwipe CurrentSwipe => ActiveSwipe > -1 && swipeWeapons != null && ActiveSwipe < swipeWeapons.Count ? swipeWeapons[ActiveSwipe] : null;
    public float CurrentSwipeEnergy => CurrentSwipe ? CurrentSwipe.CurrentEnergy : 0;
    [Space]
    protected int ActiveShake = -1;
    public List<WeaponShake> shakeWeapons = new List<WeaponShake>();
    public WeaponShake CurrentShake => ActiveShake > -1 && shakeWeapons != null && ActiveShake < shakeWeapons.Count ? shakeWeapons[ActiveShake] : null;
    public float CurrentShakeEnergy => CurrentShake ? CurrentShake.CurrentEnergy : 0;

    private Camera _camera;
    private Plane _floor;

    // ------ START METHODS ------

    protected override void OnAwake() { }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    void Start() {
        _camera = Camera.main;
        _floor = new Plane(Vector3.up, Vector3.up * floorHeight);
        _managerInput = ManagerInput.Inst;

        _managerInput.OnTouchPressInput += OnTouchPressAction;
        _managerInput.OnTouchPositionInput += SwipeAction;
        _managerInput.OnTouchReleaseInput += TouchReleaseManagerInputAction;
        _managerInput.OnShakeInput += OnShakeManagerInputAction;
    }

    // ------ ACTION METHODS ------

    void OnTouchPressAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);

        ToggleFallback(!CurrentTap || !CurrentTap.HasEnoughEnergy());
        if (_useFallback) fallbackTap.OnTouchPress(pos);
        else CurrentTap?.OnTouchPress(pos);

        CurrentSwipe?.OnTouchPress(pos);
        CurrentShake?.OnTouchPress(pos);
    }

    void SwipeAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);

        if (_useFallback) fallbackTap.OnSwipe(pos);
        else CurrentTap?.OnSwipe(pos);

        CurrentSwipe?.OnSwipe(pos);
        CurrentShake?.OnSwipe(pos);
    }

    void TouchReleaseManagerInputAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);

        if (_useFallback) fallbackTap.OnTouchRelease(pos);
        else CurrentTap?.OnTouchRelease(pos);

        CurrentSwipe?.OnTouchRelease(pos);
        CurrentShake?.OnTouchRelease(pos);
    }

    void OnShakeManagerInputAction() {
        if (_useFallback) fallbackTap.OnShake();
        else CurrentTap?.OnShake();

        CurrentSwipe?.OnShake();
        CurrentShake?.OnShake();
    }

    // ------ ADDING WEAPONS ------

    public void AddTap(DataWeaponTap d) {
        if (!Instantiate(d.prefab, transform).TryGetComponent(out WeaponTap w)) return;
        w.EventOnHit += EventOnHit;
        w.EventOnKill += EventOnKill;
        w.EventOnTap += EventOnTap;
        w.EventOnSwipe += EventOnSwipe;
        w.EventOnShake += EventOnShake;
        tapWeapons.Add(w);
        EquipTap(0);
    }
    
    public void AddSwipe(DataWeaponSwipe d) {
        if (!Instantiate(d.prefab, transform).TryGetComponent(out WeaponSwipe w)) return;
        w.EventOnHit += EventOnHit;
        w.EventOnKill += EventOnKill;
        w.EventOnTap += EventOnTap;
        w.EventOnSwipe += EventOnSwipe;
        w.EventOnShake += EventOnShake;
        swipeWeapons.Add(w);
        EquipSwipe(0);
    }
    
    public void AddShake(DataWeaponShake d) {
        if (!Instantiate(d.prefab, transform).TryGetComponent(out WeaponShake w)) return;
        w.EventOnHit += EventOnHit;
        w.EventOnKill += EventOnKill;
        w.EventOnTap += EventOnTap;
        w.EventOnSwipe += EventOnSwipe;
        w.EventOnShake += EventOnShake;
        shakeWeapons.Add(w);
        EquipShake(0);
    }

    // ------ EQUIPPING ------
    
    public void ToggleFallback(bool fallback) => EquipTap(-1, fallback);
    public void EquipTap(int slot, bool fallback = false) {
        // Do nothing if given parameters are same as before.
        if ((slot < 0 || slot == ActiveTap) && _useFallback == fallback) return;
        
        if(slot > -1) ActiveTap = slot;
        _useFallback = fallback;
        fallbackTap.ToggleWeapon(_useFallback);
        for (int i = 0; i < tapWeapons.Count; i++) tapWeapons[i].ToggleWeapon(i == ActiveTap && !_useFallback);

        if(slot > -1) EventWeaponEquip?.Invoke(CurrentTap? CurrentTap.Data : null, 0);
    }
    
    public void EquipSwipe(int slot) {
        // Do nothing if given parameters are same as before.
        if (slot < 0 || slot == ActiveSwipe) return;
        
        ActiveSwipe = slot;
        for (int i = 0; i < swipeWeapons.Count; i++) swipeWeapons[i].ToggleWeapon(i == ActiveSwipe);

        EventWeaponEquip?.Invoke(CurrentSwipe ? CurrentSwipe.Data : null, 1);
    }
    
    public void EquipShake(int slot) {
        // Do nothing if given parameters are same as before.
        if (slot < 0 || slot == ActiveShake) return;
        
        ActiveShake = slot;
        for (int i = 0; i < shakeWeapons.Count; i++) shakeWeapons[i].ToggleWeapon(i == ActiveShake);

        EventWeaponEquip?.Invoke(CurrentShake ? CurrentShake.Data : null, 2);
    }

    // ------ HELPER METHODS ------

    /// Converts screen space tap to a world point. Returns Vector3.one * -1024 if tap didn't hit ground plane.
    Vector3 TouchToWorldPoint(Vector2 screenPos) {
        // Convert screen position to world-space ray.
        Ray tapRay = _camera.ScreenPointToRay(screenPos);

        // Only continue if ray hits floor plane. Otherwise, return fallback value.
        if (!_floor.Raycast(tapRay, out float dist)) return Vector3.one * -1024;

        // Return world-space position modified by offsets.
        return tapRay.GetPoint(dist - tapOffsetTowardsCamera) + (Vector3.up * tapHeightOffset);
    }
}