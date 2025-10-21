using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour {
    private InputManager _input;
    
    [Header("Tap Point")]
    public float floorHeight = 0;
    public float tapOffsetTowardsCamera = 0;
    public float tapHeightOffset = 0.5f;

    [Header("Weapons")]
    public Transform debugTracker;
    public WeaponBase[] _playerWeapons;
    private int _currentWeapon = 0;
    
    private Camera _camera;
    private Plane _floor;

    // ------ START METHODS ------
    
    void Start() {
        _camera = Camera.main;
        _floor = new Plane(Vector3.up, Vector3.up * floorHeight);
        
        _input = InputManager.Instance;
        _input.TouchPressDelegate += TouchPressAction;
        _input.TouchPositionDelegate += SwipeAction;
        _input.TouchReleaseDelegate += TouchReleaseAction;
        _input.ShakeDelegate += ShakeAction;
    }
    
    // ------ ACTION METHODS ------

    void TouchPressAction(Vector2 input) {
        _playerWeapons[_currentWeapon].OnTouchPress(TouchToWorldPoint(input));
    }
    
    void SwipeAction(Vector2 input) {
        _playerWeapons[_currentWeapon].OnSwipe(TouchToWorldPoint(input));
    }
    
    void TouchReleaseAction(Vector2 input) {
        _playerWeapons[_currentWeapon].OnTouchRelease(TouchToWorldPoint(input));
    }
    
    void ShakeAction() {
        _playerWeapons[_currentWeapon].OnShake();
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