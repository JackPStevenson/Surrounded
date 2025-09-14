using UnityEngine;

public class Seismometer : MonoBehaviour
{
    // code from https://stackoverflow.com/questions/31389598/how-can-i-detect-a-shake-motion-on-a-mobile-device-using-unity3d-c-sharp
    // by user petem

    [SerializeField] public float shakeDetectionThreshold = 0.5f; // Lower threshold suitable for wrist shakes
    [SerializeField] public float shakeCooldownTime = 1.5f;  // Shorter cooldown period
    [SerializeField] public float requiredShakeTime = 0.3f;  // Total accumulated shake time required

    [SerializeField] private float _accelerometerUpdateInterval = 1.0f / 60.0f;
    [SerializeField] private float _lowPassKernelWidthInSeconds = 1.0f;
    [SerializeField] private float _lowPassFilterFactor;
    [SerializeField] private Vector3 _lowPassValue;

    [SerializeField] private float _timeSinceLastShake = 0;
    [SerializeField] private float _accumulatedShakeTime = 0;  // Accumulated time of shakes
    [SerializeField] private float _interruptionTolerance = 0.1f;  // Allowed time between shakes before reset
    [SerializeField] private float _timeSinceLastAcceleration = 0; // Time since last valid shake acceleration

    public delegate void ShakeDetected();
    public static event ShakeDetected OnShakeDetected;

    private void Start()
    {
        _lowPassFilterFactor = _accelerometerUpdateInterval / _lowPassKernelWidthInSeconds;
        _lowPassValue = Input.acceleration;

        OnShakeDetected += PrintShakeDetected;
    }

    private void PrintShakeDetected()
    {
        print("Shake Detected");
    }

    private void Update()
    {
        _timeSinceLastShake += Time.deltaTime;

        Vector3 acceleration = Input.acceleration;
        _lowPassValue = Vector3.Lerp(_lowPassValue, acceleration, _lowPassFilterFactor);
        Vector3 deltaAcceleration = acceleration - _lowPassValue;

        // Check for ongoing shaking above the threshold
        if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
        {
            _accumulatedShakeTime += Time.deltaTime;
            _timeSinceLastAcceleration = 0;
        }
        else
        {
            _timeSinceLastAcceleration += Time.deltaTime;
            // Only reset accumulated time if the interruption has exceeded the tolerance
            if (_timeSinceLastAcceleration > _interruptionTolerance)
            {
                _accumulatedShakeTime = 0;
            }
        }

        // Confirm shake gesture if the accumulated shaking has been sufficient and cooldown has passed
        if (_accumulatedShakeTime >= requiredShakeTime && _timeSinceLastShake >= shakeCooldownTime)
        {
            OnShakeDetected?.Invoke(); // <- the bit where we've detected a shake and fire the event; hook it up to whatever.

            _timeSinceLastShake = 0;  // Reset cooldown timer
            _accumulatedShakeTime = 0;  // Reset accumulated shake time to prevent re-triggering
        }
    }
}
