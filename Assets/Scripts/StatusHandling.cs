using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public interface IStatusHandling {
    public FloatDelegate OnStatusDamageDelegate { get; }
    public const float DoTTickDelay = 0.5f;
    protected List<StatusEffect> StatusEffects { get; }
    protected float SpeedMod { get; set; }
    protected float DamageMod { get; set; }
    protected float ResistMod { get; set; }

    public virtual void AddEffect(StatusEffect newEffect) {
        if(newEffect.Stackable) StatusEffects.Add(newEffect);
        StatusEffect e = StatusEffects.Find(e => e.IsSameEffect(newEffect));
        if (e == null) StatusEffects.Add(newEffect);
        else if(newEffect.Duration > e.GetRemainingDuration()) e.RefreshDuration(newEffect.Duration);
    }

    public virtual void ProcessEffects(float deltaTime) {
        float damageThisTick = 0;
        SpeedMod = 1;
        DamageMod = 1;
        ResistMod = 1;

        for (int i = 0; i < StatusEffects.Count; i++) {
            StatusEffect effect = StatusEffects[i];
            bool effectExpired = effect.IncrementTimer(deltaTime);

            switch (effect.Type) {
                case StatusEffectType.DamageOverTime: if (effect.CompareTag(DoTTickDelay)) damageThisTick += effect.Value; break;
                case StatusEffectType.SpeedModifier: SpeedMod *= effect.Value; break;
                case StatusEffectType.DamageModifier: DamageMod *= effect.Value; break;
                case StatusEffectType.ResistanceModifier: ResistMod *= effect.Value; break;
            }

            if (!effectExpired) continue;
            StatusEffects.RemoveAt(i);
            i--;
        }
        
        OnStatusDamageDelegate?.Invoke(damageThisTick);
    }
}

public class StatusEffect {
    // --- DATA ENTRY ---
    private StatusEffectData _data;
    
    // --- DATA REFERENCES ---
    public string EffectName => _data.effectName;
    public int EffectNameHash;
    public float Duration => _data.duration;
    public bool Stackable => _data.stackable;
    
    public StatusEffectType Type => _data.type;
    public float Value => _data.value;
    
    // --- CURRENT STATE ---
    public float ElapsedTime { get; private set; }
    private float TagTimer; // Used for timing things like a damage over time effect's damage ticks.

    // --- START METHODS ---
    
    public StatusEffect(StatusEffectData data) {
        _data = data;
        EffectNameHash = data.effectName.GetHashCode();
    }
    
    // --- EVENT METHODS ---
    
    /// Increments effect's timer. Returns true if elapsed time has reached full duration.
    public bool IncrementTimer(float deltaTime) {
        ElapsedTime += deltaTime;
        TagTimer += deltaTime;
        return ElapsedTime >= Duration;
    }
    
    // Compares current time to last tagged time (0 on start). Returns true and updates last tag time if delta exceeds given value.
    public bool CompareTag(float tagTimeCheck) {
        if (TagTimer < tagTimeCheck) return false;
        TagTimer = 0;
        return true;
    }
    
    /// Refreshes effect's duration. Optionally sets effect's current duration to given value. 
    public void RefreshDuration(float customDuration = -1) => ElapsedTime = customDuration >= 0 ? Duration - customDuration : 0;
    
    // --- HELPER METHODS ---
    
    /// Returns whether given effect is the same as this one (via name comparison).
    public bool IsSameEffect(StatusEffect other) => other.EffectNameHash == EffectNameHash;

    public float GetRemainingDuration() => Duration - ElapsedTime;
}