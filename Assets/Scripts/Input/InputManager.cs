using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour {
    public static InputManager Instance;

    // General
    public bool IsTouching { get; private set; }
    public Vector2 LastTouchPosition { get; private set; }
    public Vector2Delegate TouchPressDelegate;
    public Vector2Delegate TouchReleaseDelegate;
    public Vector2Delegate TouchPositionDelegate;
    
    // Swiping
    public Vector2 SwipeVector { get; private set; }
    public Vector2 SwipeDirection { get { return SwipeVector.normalized; } }
    public float SwipeDistance { get { return SwipeVector.magnitude; } }
    public Vector2Delegate SwipeDelegate;
    
    // Shaking
    public Vector3 ShakeVector { get; private set; }
    public float ShakeStrength { get { return ShakeVector.magnitude; } }
    public Vector3Delegate ShakeDelegate;
    
    // ------ START METHODS ------

    private void Awake() {
        Instance = this;
    }

    // ------ EVENT METHODS ------

    private void OnTouchPress(InputValue value) {
        IsTouching = true;
        TouchPressDelegate?.Invoke(LastTouchPosition);
    }
    
    private void OnTouchPosition(InputValue value) {
        LastTouchPosition = value.Get<Vector2>();
        TouchPositionDelegate?.Invoke(LastTouchPosition);
    }
    
    private void OnTouchRelease(InputValue value) {
        IsTouching = false;
        TouchReleaseDelegate?.Invoke(LastTouchPosition);
    }

    private void OnSwipe(InputValue value) {
        SwipeVector = value.Get<Vector2>();
        SwipeDelegate?.Invoke(SwipeVector);
    }

    private void OnShake(InputValue value) {
        ShakeVector = value.Get<Vector3>();
        ShakeDelegate?.Invoke(ShakeVector);
    }
    
}