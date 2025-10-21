using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusHandler : MonoBehaviour, IUpdateCustom {
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

    public void FixedUpdateCustom(float deltaTime, int tick = 0) {
        float[] newValues = {0, 1, 1, 1};

        for (int i = _statusEffects.Count - 1; i >= 0; i--) {
            StatusEffect effect = _statusEffects[i];
            bool expired = effect.IncrementTimer(deltaTime);
            
            // Apply effect's value to corresponding status.
            if (effect.Type == 0) newValues[0] += effect.TagElapsed(0.5f) ? effect.Value : 0;
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
    
    public void AddEffect(DataStatusEffect newEffect) {
        if (newEffect.stackable) _statusEffects.Add(new StatusEffect(newEffect));
        else {
            StatusEffect existing = TryGetEffect(newEffect);
            if(existing.IsEmpty) _statusEffects.Add(new StatusEffect(newEffect));
            else if (newEffect.duration > existing.TimeLeft) existing.Refresh(newEffect.duration);
        }
    }
    
    public void Reset() {
        _statusEffects.Clear();
    }
    
    // ------ HELPER METHODS ------

    StatusEffect TryGetEffect(DataStatusEffect effect) => _statusEffects.Find(e => e.Compare(effect));
}

public struct StatusEffect {
    // --- DATA ---
    public DataStatusEffect Data { get; private set; }
    public string Name => Data.effectName;
    public float Duration => Data.duration;
    public StatusEffectType Type => Data.type;
    public float Value => Data.value;
    public bool IsEmpty { get; private set; }
    
    // --- CURRENT STATE ---
    public float ElapsedTime { get; private set; }
    public float TimeLeft => Duration - ElapsedTime;
    private float TagTimer; // Used for timing things like a mod health over time effect's ticks.

    // --- CONSTRUCTORS ---

    public StatusEffect(DataStatusEffect data) {
        Data = data;
        IsEmpty = !data;
        ElapsedTime = 0;
        TagTimer = 0;
    }
    
    // --- EVENT METHODS ---
    
    /// Increments effect's timer. Returns true if elapsed time has reached full duration.
    public bool IncrementTimer(float deltaTime) {
        ElapsedTime += deltaTime;
        TagTimer += deltaTime;
        return ElapsedTime >= Duration;
    }
    
    /// Refreshes duration. Optionally sets effect's current duration to given value. 
    public void Refresh(float newDuration = -1) => ElapsedTime = newDuration >= 0 ? Duration - newDuration : 0;
    
    // --- HELPER METHODS ---
    
    // Checks if tag timer exceeds threshold. Returns true and resets tag timer if delta exceeds given value.
    public bool TagElapsed(float timeThreshold) {
        if (TagTimer < timeThreshold) return false;
        TagTimer = 0;
        return true;
    }
    
    /// Returns whether given effect is the same as this one (via name comparison).
    public bool Compare(DataStatusEffect other) => other.name == Name;

}