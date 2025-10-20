using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour {
    public readonly static float MinSwipeDistance = 0.75f;
    private InputManager _input;
    
    [Header("Tapping")]
    public float floorHeight = 0;
    public float tapOffsetTowardsCamera = 0;
    public float tapHeightOffset = 0.5f;
    
    [Header("Weapons")]
    public int activeTapWeapon = 0;
    public List<WeaponTap> tapWeapons;
    public WeaponTap CurrentTap => (activeTapWeapon > -1 && activeTapWeapon < tapWeapons.Count) ? tapWeapons[activeTapWeapon] : null;
    [Space]
    public int activeSwipeWeapon = 0;
    public List<WeaponSwipe> swipeWeapons;
    public WeaponSwipe CurrentSwipe => (activeSwipeWeapon > -1 && activeSwipeWeapon < swipeWeapons.Count) ? swipeWeapons[activeSwipeWeapon] : null;
    [Space]
    public int activeShakeWeapon = 0;
    public List<WeaponShake> shakeWeapons;
    public WeaponShake CurrentShake => (activeShakeWeapon > -1 && activeShakeWeapon < shakeWeapons.Count) ? shakeWeapons[activeShakeWeapon] : null;
    
    private Camera _camera;
    private Plane _floor;

    // ------ START METHODS ------
    
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
        
        CurrentTap.OnTouchPress(pos);
        CurrentSwipe.OnTouchPress(pos);
        CurrentShake.OnTouchPress(pos);
    }
    
    void SwipeAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);
        
        CurrentTap.OnSwipe(pos);
        CurrentSwipe.OnSwipe(pos);
        CurrentShake.OnSwipe(pos);
    }
    
    void TouchReleaseInputAction(Vector2 input) {
        Vector3 pos = TouchToWorldPoint(input);
        
        CurrentTap.OnTouchRelease(pos);
        CurrentSwipe.OnTouchRelease(pos);
        CurrentShake.OnTouchRelease(pos);
    }
    
    void OnShakeInputAction() {
        CurrentTap.OnShake();
        CurrentSwipe.OnShake();
        CurrentShake.OnShake();
    }
    
    // ------ EQUIPPING ------

    private void EquipWeaponsInternal() {
        for (int i = 0; i < tapWeapons.Count; i++)
            tapWeapons[i].ToggleWeapon(i == activeTapWeapon);
        for (int i = 0; i < swipeWeapons.Count; i++)
            swipeWeapons[i].ToggleWeapon(i == activeTapWeapon);
        for (int i = 0; i < shakeWeapons.Count; i++)
            shakeWeapons[i].ToggleWeapon(i == activeTapWeapon);
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