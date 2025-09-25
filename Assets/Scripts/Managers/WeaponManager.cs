using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour {
    private InputManager _input;
    
    [Header("Physics")]
    public LayerMask hitMask;
    public float tapDistanceFromCamera = 10;

    [Header("Weapons")]
    public Transform debugTracker;
    private WeaponBase[] _playerWeapons;
    private int _currentWeapon = 0;
    
    private Camera _camera;

    void Start() {
        _camera = Camera.main;
        
        _input = InputManager.Instance;
        _input.TouchPressDelegate += TouchPressAction;
        _input.TouchPositionDelegate += SwipeAction;
        _input.TouchReleaseDelegate += TouchReleaseAction;
    }

    void Update() {

    }


    void TouchPressAction(Vector2 input) {
        _playerWeapons[_currentWeapon].OnTouchPress(TouchToWorldPoint(input));
    }
    
    void SwipeAction(Vector2 input) {
        _playerWeapons[_currentWeapon].OnSwipe(TouchToWorldPoint(input));
    }
    
    void TouchReleaseAction(Vector2 input) {
        _playerWeapons[_currentWeapon].OnTouchRelease(TouchToWorldPoint(input));
    }

    /// Converts a point on screen to a world point
    Vector3 TouchToWorldPoint(Vector2 screenPos) {
        // Convert screen position to viewport position with set distance away from camera.
        Vector3 viewPos = _camera.ScreenToViewportPoint(screenPos);
        viewPos.z = tapDistanceFromCamera;
        
        // Return world position from converted view position.
        return _camera.ViewportToWorldPoint(viewPos);
    }
}