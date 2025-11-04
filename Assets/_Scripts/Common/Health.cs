using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour {
    public event Action<float> OnHealthChange;
    public event Action OnDeath;
    
    [FormerlySerializedAs("maximum")] [Header("General")]
    public float HpMax;
    public float HpCurrent { get; private set; }
    public bool destroyOnDeath = false;
    
    public Vector3 Position => transform.position;

    // ------ START METHODS ------
    
    void Start() {
        HpCurrent = HpMax;
    }
    
    // ------ EVENT METHODS ------
    
    public void Reset() => HpCurrent = HpMax;

    public float DealDamage(float damage) {
        if (Mathf.Approximately(damage, 0)) return HpCurrent;
        
        HpCurrent -= damage;
        OnHealthChange?.Invoke(-damage);

        // If health reaches 0, invoke death event.
        if (HpCurrent > 0) return HpCurrent;
        OnDeath?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
        return 0;
    }

    public void SetMaxHealth(float newMax, bool scaleCurrent = true, bool reset = false) {
        HpMax = newMax;

        if (scaleCurrent) HpCurrent = newMax/HpMax;
        if (reset) Reset();
    }
}