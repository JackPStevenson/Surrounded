using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusHandling : MonoBehaviour, IUpdateCustom {
    public event Action<float> OnModHealthOverTime;
    public event Action<StatusEffectType, float> OnEffectApplied;
    
    // --- EFFECTS AND AFFECTED PARAMETERS ---
    private readonly List<StatusEffect> _statusEffects = new List<StatusEffect>();
    private readonly float[] _statusValues = {0, 1, 1, 1};
    public float SpeedMod { get => _statusValues[1]; private set => _statusValues[1] = value; }
    public float DamageMod { get => _statusValues[2]; private set => _statusValues[2] = value; }
    public float ResistMod { get => _statusValues[3]; private set => _statusValues[3] = value; }

    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) { }

    public void FixedUpdateCustom(int tick, float deltaTime) {
        float[] newValues = {0, 1, 1, 1};

        for (int i = _statusEffects.Count - 1; i >= 0; i--) {
            StatusEffect effect = _statusEffects[i];
            bool expired = effect.IncrementTimer(deltaTime);
            
            // Apply effect's value to corresponding status.
            if (effect.Type == 0) newValues[0] += effect.CompareLastTag(0.5f) ? effect.Value : 0;
            else newValues[(int) effect.Type] *= effect.Value;

            if (expired) _statusEffects.RemoveAt(i);
        }

        // Invoke effect applied event for effects whose values have changed.
        if (!Mathf.Approximately(newValues[0], 0)) {
            OnModHealthOverTime?.Invoke(newValues[0]);
            OnEffectApplied?.Invoke(0, newValues[0]);
        }
        for (int i = 1; i < 4; i++) {
            if (Mathf.Approximately(newValues[i], _statusValues[i])) continue;
            _statusValues[i] = newValues[i];
            OnEffectApplied?.Invoke((StatusEffectType) i, _statusValues[i]);
        }
    }

    // ------ EVENT METHODS ------
    
    public void AddEffect(StatusEffectData data) {
        if (data.stackable || !HasEffect(data, out StatusEffect found)) _statusEffects.Add(new StatusEffect(data));
        else if (data.duration > found.GetTimeLeft()) found.Refresh(data.duration);
    }
    
    public void Reset() {
        _statusEffects.Clear();
    }
    
    // ------ HELPER METHODS ------

    bool HasEffect(StatusEffectData effectData, out StatusEffect foundEffect) {
        foundEffect = _statusEffects.Find(e => e.IsSameEffect(effectData));
        return foundEffect != null;
    }
}

public class StatusEffect {
    // --- DATA ENTRY ---
    private StatusEffectData _data;
    
    // --- DATA REFERENCES ---
    public string EffectName => _data.effectName;
    public float Duration => _data.duration;
    public bool Stackable => _data.stackable;
    private readonly int EffectNameHash;
    
    public StatusEffectType Type => _data.type;
    public float Value => _data.value;
    
    // --- CURRENT STATE ---
    public float ElapsedTime { get; private set; }
    private float TagTimer; // Used for timing things like a mod health over time effect's ticks.

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
    public bool CompareLastTag(float tagTimeCheck) {
        if (TagTimer < tagTimeCheck) return false;
        TagTimer = 0;
        return true;
    }
    
    /// Refreshes effect's duration. Optionally sets effect's current duration to given value. 
    public void Refresh(float customDuration = -1) => ElapsedTime = customDuration >= 0 ? Duration - customDuration : 0;
    
    // --- HELPER METHODS ---
    
    /// Returns whether given effect is the same as this one (via name comparison).
    public bool IsSameEffect(StatusEffect other) => other.EffectNameHash == EffectNameHash;
    public bool IsSameEffect(StatusEffectData other) => other.name.GetHashCode() == EffectNameHash;

    public float GetTimeLeft() => Duration - ElapsedTime;
}