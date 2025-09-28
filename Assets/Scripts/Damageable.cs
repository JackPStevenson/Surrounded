using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {
    public event FloatFloatDelegate OnDamaged;
    public event GenericDelegate OnDeath;

    public Vector3 Position { get => transform.position; set => transform.position = value; }

    [Header("General")]
    public float maxHealth;
    protected float CurrentHealth;

    // ------ START METHODS ------
    
    void Start() {
        CurrentHealth = maxHealth;
        OnStart();
    }

    protected virtual void OnStart(){}
    
    // ------ EVENT METHODS ------

    /// Deals damage based on given value. If damageable's health drops below 0, OnDeath will be invoked.
    public void DealDamage(float damage) {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnDamaged?.Invoke(damage, CurrentHealth);
        
        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
    }

    public void Kill() {
        CurrentHealth = 0;
        OnDeath?.Invoke();
    }
}