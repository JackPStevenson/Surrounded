using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {
    public delegate void DamageDelegate(float damage, float healthLeft);
    public event DamageDelegate OnDamaged;
    public delegate void DeathDelegate();
    public event DeathDelegate OnDeath;

    public Vector3 Position {
        get => transform.position;
        set => transform.position = value;
    }

    [Header("General")]
    public float maxHealth;
    protected float CurrentHealth;
    
    protected void Start() {
        CurrentHealth = maxHealth;
    }

    public void DealDamage(float damage) {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnDamaged?.Invoke(damage, CurrentHealth);
        
        if(CurrentHealth <= 0) OnDeath?.Invoke();
    }
}