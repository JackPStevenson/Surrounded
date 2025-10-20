using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class InputManager : MonoBehaviour {
    public static InputManager Instance;

    // General
    public bool IsTouching { get; private set; }
    public Vector2 LastTouchPosition { get; private set; }
    public event Action<Vector2> OnTouchPressInput;
    public event Action<Vector2> OnTouchReleaseInput;
    public event Action<Vector2> OnTouchPositionInput;
    public event Action OnShakeInput;
    
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
    
    // ------ START METHODS ------

    private void Awake() {
        Instance = this;
        
        _accel = Accelerometer.current;
        
        if (_accel == null) return;
        InputSystem.EnableDevice(_accel);
        _accel.samplingFrequency = 60f;

        _lowPassValue = Vector3.up;
    }

    void Update() {
        // Send Messages doesn't seem to work for accelerometer. This serves as a crude workaround.
        if (_accel != null) OnShakeCustom(_accel.acceleration.ReadValue());
    }

    // ------ EVENT METHODS ------

    private void OnTouchPress(InputValue value) {
        IsTouching = true;
        OnTouchPressInput?.Invoke(LastTouchPosition);
    }
    
    private void OnTouchPosition(InputValue value) {
        LastTouchPosition = value.Get<Vector2>();
        OnTouchPositionInput?.Invoke(LastTouchPosition);
    }
    
    private void OnTouchRelease(InputValue value) {
        IsTouching = false;
        OnTouchReleaseInput?.Invoke(LastTouchPosition);
    }

    private void OnShakeDebug(InputValue value) {
        print(1);
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