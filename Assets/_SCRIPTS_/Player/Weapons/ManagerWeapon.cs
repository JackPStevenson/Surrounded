using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagerWeapon : MonoBehaviour {
    public event Action<DataWeapon, int> EventWeaponEquip;
    public static ManagerWeapon Instance;
    private ManagerInput _managerInput;
    
    [Header("Tapping")]
    public float floorHeight = 0;
    public float tapOffsetTowardsCamera = 0;
    public float tapHeightOffset = 0.5f;
    
    [Header("Weapons")]
    public WeaponTap fallbackTap;
    public int activeTap = 0;
    private bool _useFallback = false;
    public List<WeaponTap> tapWeapons = new List<WeaponTap>();
    public WeaponTap CurrentTap => activeTap > -1 && tapWeapons != null && activeTap < tapWeapons.Count ? tapWeapons[activeTap] : null;
    public DataWeaponTap CurrentTapData => CurrentTap ? CurrentTap.Data : null;
    [Space]
    public int activeSwipe = 0;
    public List<WeaponSwipe> swipeWeapons = new List<WeaponSwipe>();
    public WeaponSwipe CurrentSwipe => activeSwipe > -1 && swipeWeapons != null && activeSwipe < swipeWeapons.Count ? swipeWeapons[activeSwipe] : null;
    public DataWeaponSwipe CurrentSwipeData => CurrentSwipe ? CurrentSwipe.Data : null;
    [Space]
    public int activeShake = 0;
    public List<WeaponShake> shakeWeapons = new List<WeaponShake>();
    public WeaponShake CurrentShake => activeShake > -1 && shakeWeapons != null && activeShake < shakeWeapons.Count ? shakeWeapons[activeShake] : null;
    public DataWeaponShake CurrentShakeData => CurrentShake ? CurrentShake.Data : null;
    
    private Camera _camera;
    private Plane _floor;

    // ------ START METHODS ------

    void Awake() {
        Instance = this;
    }

    void Start() {
        _camera = Camera.main;
        _floor = new Plane(Vector3.up, Vector3.up * floorHeight);
        _managerInput = ManagerInput.Instance;
        
        _managerInput.OnTouchPressInput += OnTouchPressAction;
        _managerInput.OnTouchPositionInput += SwipeAction;
        _managerInput.OnTouchReleaseInput += TouchReleaseManagerInputAction;
        _managerInput.OnShakeInput += OnShakeManagerInputAction;
    }
    
    // ------ ACTION METHODS ------

    void OnTouchPressAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);

        ToggleFallback(!CurrentTap || !CurrentTap.HasEnoughEnergy());
        if(_useFallback) fallbackTap.OnTouchPress(pos);
        else CurrentTap?.OnTouchPress(pos);
        
        CurrentSwipe?.OnTouchPress(pos);
        CurrentShake?.OnTouchPress(pos);
    }
    
    void SwipeAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);
        
        if(_useFallback) fallbackTap.OnSwipe(pos);
        else CurrentTap?.OnSwipe(pos);
        
        CurrentSwipe?.OnSwipe(pos);
        CurrentShake?.OnSwipe(pos);
    }
    
    void TouchReleaseManagerInputAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);
        
        if(_useFallback) fallbackTap.OnTouchRelease(pos);
        else CurrentTap?.OnTouchRelease(pos);
        
        CurrentSwipe?.OnTouchRelease(pos);
        CurrentShake?.OnTouchRelease(pos);
    }
    
    void OnShakeManagerInputAction() {
        if(_useFallback) fallbackTap.OnShake();
        else CurrentTap?.OnShake();
        
        CurrentSwipe?.OnShake();
        CurrentShake?.OnShake();
    }

    // ------ ADDING WEAPONS ------

    public void AddTap(DataWeaponTap d) {
        if (Instantiate(d.prefab, transform).TryGetComponent(out WeaponTap w)) {
            tapWeapons.Add(w);
            EquipTap(0);
        } }
    public void AddSwipe(DataWeaponSwipe d) {
        if (Instantiate(d.prefab, transform).TryGetComponent(out WeaponSwipe w)) {
            swipeWeapons.Add(w);
            EquipSwipe(0);
        } }
    public void AddShake(DataWeaponShake d) {
        if (Instantiate(d.prefab, transform).TryGetComponent(out WeaponShake w)) {
            shakeWeapons.Add(w);
            EquipShake(0);
        } }
    
    // ------ EQUIPPING ------

    public void EquipTap(int slot) => EquipWeaponsInternal(slot, -1, -1, _useFallback);
    public void EquipSwipe(int slot) => EquipWeaponsInternal(-1, slot, -1, _useFallback);
    public void EquipShake(int slot) => EquipWeaponsInternal(-1, -1, slot, _useFallback);
    public void ToggleFallback(bool fallback) => EquipWeaponsInternal(-1, -1, -1, fallback);

    private void EquipWeaponsInternal(int tap, int swipe, int shake, bool fallback) {
        if (tap > -1) activeTap = tap;
        if (swipe > -1) activeSwipe = swipe;
        if (shake > -1) activeShake = shake;

        _useFallback = fallback;
        fallbackTap.ToggleWeapon(_useFallback);
        
        for (int i = 0; i < tapWeapons.Count; i++) tapWeapons[i]?.ToggleWeapon(i == activeTap && !_useFallback);
        for (int i = 0; i < swipeWeapons.Count; i++) swipeWeapons[i]?.ToggleWeapon(i == activeSwipe);
        for (int i = 0; i < shakeWeapons.Count; i++) shakeWeapons[i]?.ToggleWeapon(i == activeShake);

        EventWeaponEquip?.Invoke(CurrentTap.Data, 0);
        EventWeaponEquip?.Invoke(CurrentSwipe.Data, 1);
        EventWeaponEquip?.Invoke(CurrentShake.Data, 2);
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