using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour {
    private readonly static int LastDamageFlash = Shader.PropertyToID("_Last_Damage_Flash");
    public event FloatFloatDelegate OnDamaged;
    public event GenericDelegate OnDeath;

    public Vector3 Position { get => transform.position; set => transform.position = value; }

    [Header("References")]
    public MeshRenderer[] renderers;
    public SkinnedMeshRenderer[] skinnedRenderers;
    
    [Header("General")]
    public float maxHealth;
    protected float CurrentHealth;

    // ------ START METHODS ------
    
    void Start() {
        CurrentHealth = maxHealth;
        OnStart();
    }

    protected virtual void OnStart(){}
    
    // ------ EVENT METHODS ------as

    /// Deals damage based on given value. If damageable's health drops below 0, OnDeath will be invoked. Returns remaining health.
    public float DealDamage(float damage) {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        OnDamaged?.Invoke(damage, CurrentHealth);

        if(renderers.Length > 0)
            foreach (MeshRenderer r in renderers)
                r.material.SetFloat(LastDamageFlash, Time.time);
        
        if(skinnedRenderers.Length > 0)
            foreach (SkinnedMeshRenderer r in skinnedRenderers)
                r.material.SetFloat(LastDamageFlash, Time.time);
        
        if (CurrentHealth <= 0)
            OnDeath?.Invoke();
        
        return CurrentHealth;
    }

    public void Kill() {
        CurrentHealth = 0;
        OnDeath?.Invoke();
    }
}