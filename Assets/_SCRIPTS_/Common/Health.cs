using System;
using UnityEngine;

public class Health : MonoBehaviour {
    public event Action<float> EventHealthChange;
    public event Action EventDeath;
    
    [Header("General")]
    public float healthMax;
    protected float HealthCurrentRatio = 1;
    public float HealthCurrent => healthMax * HealthCurrentRatio;
    public float HealthRatio => HealthCurrentRatio;
    public bool destroyOnDeath = false;
    
    public Vector3 Position => transform.position;
    
    public void SetStatusHandler(StatusHandler handler) => _status = handler;
    private StatusHandler _status;
    
    // ------ EVENT METHODS ------
    
    public void Reset() => HealthCurrentRatio = 1;

    public float DealDamage(float damage) {
        // If health has attached status effect, apply current resistance to incoming damage.
        float modifiedDmg = GetModifiedDamage(damage);
        if (Mathf.Approximately(modifiedDmg, 0)) return HealthCurrent;
    
        float damageToRatio = modifiedDmg / GetModifiedMaxHealth();
        HealthCurrentRatio -= damageToRatio;
        EventHealthChange?.Invoke(-modifiedDmg);

        // If health reaches 0, invoke death event.
        if (HealthCurrent > 0) return HealthCurrent;
        EventDeath?.Invoke();
        if (destroyOnDeath) Destroy(gameObject);
        return 0;
    }

    public void SetMaxHealth(float newMax, bool resetCurrent = false) {
        healthMax = newMax;
        if (resetCurrent) Reset();
    }

    float GetModifiedDamage(float dmg) => (dmg <= 0) ? dmg : ((_status) ? _status.ModConst(AffectorConstType.Resistance, dmg, true, true) : dmg);
    float GetModifiedMaxHealth() => (_status) ? _status.ModConst(AffectorConstType.MaxHealth, healthMax) : healthMax;
}