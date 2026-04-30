using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.Serialization;

public class ManagerInput : MonoSingleton<ManagerInput> {
    public readonly static float MinSwipeDistance = 0.75f;

    // General
    public bool IsTouching { get; private set; }
    public Vector2 TouchPos { get; private set; }
    public event Action<Vector2> OnTouchPressInput;
    public event Action<Vector2> OnTouchReleaseInput;
    public event Action<Vector2> OnTouchPositionInput;
    public event Action OnShakeInput;
    [Header("Swipe Input")]
    
    // Shaking    
    [Header("Shake Input")]
    public float shakePowerThreshold = 0.5f; // Lower threshold suitable for wrist shakes.
    public float shakeDurationRequirement = 0.3f;  // Total accumulated shake time required.
    public float shakeCooldownTime = 1.5f;  // Cooldown between each shake.

    public float shakeInterruptionTolerance = 0.1f;  // Allowed time between shakes before reset
    [Range(1, 6)]
    public float lowPassFilterFactor = 3.55f; // How quickly low pass value converges on 
    private Vector3 _lowPassValue;

    private float _accumulatedShakeTime = 0;  // Accumulated time of shakes.
    private float _lastShakeTime = 0; // Time since last full shake.
    private float _lastAccelerationTime = 0; // Time since last valid shake acceleration.

    private Accelerometer _accel;

    private SE_InputAction _controls;
    
    // ------ START METHODS ------

    protected override void OnAwake() {
        _lowPassValue = Vector3.up;
        
        _accel = Accelerometer.current;
        if (_accel == null) return;
        InputSystem.EnableDevice(_accel);
        _accel.samplingFrequency = 60f;
    }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    // ------ UPDATE METHODS ------

    void Update() {
        bool IsGameOver = ManagerGame.Inst && ManagerGame.Inst.GetGameState() is GameState.Dead;
        bool IsPaused = Time.timeScale <= 0;
        if (IsGameOver || IsPaused) {
            if (IsTouching) {
                OnTouchReleaseInput?.Invoke(TouchPos);
                IsTouching = false;
            }

            return;
        }
        
        Mouse mouse = Mouse.current;
        if (mouse != null && (mouse.leftButton.isPressed || mouse.leftButton.wasReleasedThisFrame))
            UpdateTouch(mouse.leftButton.wasPressedThisFrame, mouse.leftButton.isPressed, mouse.position.ReadValue());
        
        TouchControl touch = (Touchscreen.current != null) ? Touchscreen.current.primaryTouch : null;
        if (touch != null && (touch.press.isPressed || touch.press.wasReleasedThisFrame))
            UpdateTouch(touch.press.wasPressedThisFrame, touch.press.isPressed, touch.position.ReadValue());
        
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null && (keyboard.leftShiftKey.wasPressedThisFrame))
            OnShakeDebug();
        
        if (_accel != null)
            OnShakeCustom(_accel.acceleration.ReadValue());
    }

    private void UpdateTouch(bool wasPressedThisFrame, bool isPressed, Vector2 touchPos) {
        TouchPos = touchPos;

        // Only enable touching if input was activated this frame (prevents multi-input conflicts).
        if (wasPressedThisFrame) {
            IsTouching = true;
            OnTouchPressInput?.Invoke(TouchPos);
        }
        else if (IsTouching) {
            if (isPressed) {
                OnTouchPositionInput?.Invoke(TouchPos);
            }
            else {
                OnTouchReleaseInput?.Invoke(TouchPos);
                IsTouching = false;
            }
        }
    }

    // ------ EVENT METHODS ------

    public void OnShakeDebug() {
        // Invoke shake delegate and reset timers.
        OnShakeInput?.Invoke();
        _lastShakeTime = Time.time;
        _accumulatedShakeTime = 0;
    }

    // Shake code by Petem: https://stackoverflow.com/questions/31389598/how-can-i-detect-a-shake-motion-on-a-mobile-device-using-unity3d-c-sharp
    private void OnShakeCustom(Vector3 acceleration) {
        print(acceleration + " " + _lowPassValue);
        // Gradually adjust low pass value. If acceleration deviates enough from it, start accumulating shake time.
        _lowPassValue = Common.SmoothLerp(_lowPassValue, acceleration, Mathf.Pow(0.1f, lowPassFilterFactor), Time.deltaTime);
        if ((acceleration - _lowPassValue).sqrMagnitude >= shakePowerThreshold) {
            _accumulatedShakeTime += Time.deltaTime;
            _lastAccelerationTime = Time.time;
        }
        // Otherwise, reset accumulated time if the interruption duration has exceeded tolerance.
        else if (Time.time > _lastAccelerationTime + shakeInterruptionTolerance) {
            _accumulatedShakeTime = 0;
        }

        // Only proceed if enough shaking has been accumulated and cooldown has passed. 
        if (!(_accumulatedShakeTime >= shakeDurationRequirement) || !(Time.time >= shakeCooldownTime + _lastShakeTime)) return;
        
        // Invoke shake delegate and reset timers.
        OnShakeInput?.Invoke();
        _lastShakeTime = Time.time;
        _accumulatedShakeTime = 0;
    }


    private void OnApplicationFocus(bool hasFocus) {
        if (hasFocus || !IsTouching)
            return;
        
        OnTouchReleaseInput?.Invoke(TouchPos);
        IsTouching = false;
    }
}