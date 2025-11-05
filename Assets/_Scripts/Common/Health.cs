using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Health : MonoBehaviour {
    public event Action<float> OnHealthChange;
    public event Action OnDeath;
    
    [Header("General")]
    public float healthMax;
    protected float HealthCurrentRatio = 1;
    public float HealthCurrent => healthMax * HealthCurrentRatio;
    public bool destroyOnDeath = false;
    
    public Vector3 Position => transform.position;
    
    // ------ EVENT METHODS ------
    
    public void Reset() => HealthCurrentRatio = 1;

    public float DealDamage(float damage) {
        if (Mathf.Approximately(damage, 0)) return HealthCurrent;

        float damageToRatio = damage / healthMax;
        HealthCurrentRatio -= damageToRatio;
        OnHealthChange?.Invoke(-damage);

        // If health reaches 0, invoke death event.
        if (HealthCurrent > 0) return HealthCurrent;
        OnDeath?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
        return 0;
    }

    public void SetMaxHealth(float newMax, bool resetCurrent = false) {
        healthMax = newMax;
        if (resetCurrent) Reset();
    }
}