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
        if (Touchscreen.current == null)
            return;

        
        TouchControl touch = Touchscreen.current.primaryTouch;
        TouchPressControl touchPress = touch.press;
        TouchPos = touch.position.ReadValue();

        if (touchPress.wasPressedThisFrame) {
            IsTouching = true;
            OnTouchPressInput?.Invoke(TouchPos);
        }
        else if (IsTouching) {
            if (touchPress.isPressed) {
                OnTouchPositionInput?.Invoke(TouchPos);
            }
            else {
                OnTouchReleaseInput?.Invoke(TouchPos);
                IsTouching = false;
            }
        }

        // Send Messages doesn't seem to work for accelerometer. This serves as a crude workaround.
        if (_accel != null) OnShakeCustom(_accel.acceleration.ReadValue());
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
    
}