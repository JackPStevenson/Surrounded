using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour {
    public static WeaponManager Instance;
    public readonly static float MinSwipeDistance = 0.75f;
    private InputManager _input;
    
    [Header("Tapping")]
    public float floorHeight = 0;
    public float tapOffsetTowardsCamera = 0;
    public float tapHeightOffset = 0.5f;
    
    [Header("Weapons")]
    public WeaponTap fallbackTap;
    public int activeTap = 0;
    private bool _useFallback = false;
    public List<WeaponTap> tapWeapons;
    public WeaponTap CurrentTap => activeTap > -1 && activeTap < tapWeapons.Count ? tapWeapons[activeTap] : null;
    [Space]
    public int activeSwipe = 0;
    public List<WeaponSwipe> swipeWeapons;
    public WeaponSwipe CurrentSwipe => activeSwipe > -1 && activeSwipe < swipeWeapons.Count ? swipeWeapons[activeSwipe] : null;
    [Space]
    public int activeShake = 0;
    public List<WeaponShake> shakeWeapons;
    public WeaponShake CurrentShake => activeShake > -1 && activeShake < shakeWeapons.Count ? shakeWeapons[activeShake] : null;
    
    private Camera _camera;
    private Plane _floor;

    // ------ START METHODS ------

    void Awake() {
        Instance = this;
    }

    void Start() {
        _camera = Camera.main;
        _floor = new Plane(Vector3.up, Vector3.up * floorHeight);
        _input = InputManager.Instance;
        
        _input.OnTouchPressInput += OnTouchPressAction;
        _input.OnTouchPositionInput += SwipeAction;
        _input.OnTouchReleaseInput += TouchReleaseInputAction;
        _input.OnShakeInput += OnShakeInputAction;
    }
    
    // ------ ACTION METHODS ------

    void OnTouchPressAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);

        ToggleFallback(!CurrentTap.HasEnoughEnergy());
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
    
    void TouchReleaseInputAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);
        
        if(_useFallback) fallbackTap.OnTouchRelease(pos);
        else CurrentTap?.OnTouchRelease(pos);
        
        CurrentSwipe?.OnTouchRelease(pos);
        CurrentShake?.OnTouchRelease(pos);
    }
    
    void OnShakeInputAction() {
        if(_useFallback) fallbackTap.OnShake();
        else CurrentTap?.OnShake();
        
        CurrentSwipe?.OnShake();
        CurrentShake?.OnShake();
    }
    
    // ------ ADDING WEAPONS ------

    public void AddTap(GameObject prefab) { if(Instantiate(prefab, transform).TryGetComponent(out WeaponTap w)) tapWeapons.Add(w); }
    public void AddSwipe(GameObject prefab) { if(Instantiate(prefab, transform).TryGetComponent(out WeaponSwipe w)) swipeWeapons.Add(w); }
    public void AddShake(GameObject prefab) { if(Instantiate(prefab, transform).TryGetComponent(out WeaponShake w)) shakeWeapons.Add(w); }
    
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
        
        for (int i = 0; i < tapWeapons.Count; i++) tapWeapons[i].ToggleWeapon(i == activeTap && !_useFallback);
        for (int i = 0; i < swipeWeapons.Count; i++) swipeWeapons[i].ToggleWeapon(i == activeSwipe);
        for (int i = 0; i < shakeWeapons.Count; i++) shakeWeapons[i].ToggleWeapon(i == activeShake);
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