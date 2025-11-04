using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


public struct StatusEffect : IUpdateCustom {
    public const float EffectTickInterval = 0.5f;
    public bool IsEmpty { get; private set; }
    
    // --- GENERAL ---
    public readonly DataStatusEffect Data;
    public readonly string Name => Data.name;
    
    // --- EFFECTS ---
    public float Potency;
    public AffectorConstant[] ConstAffectors => Data.constantAffectors is {Length: > 0} ? Data.constantAffectors : null;
    public AffectorDynamic[] DynamicAffectors => Data.dynamicAffectors is {Length: > 0} ? Data.dynamicAffectors : null;
    public readonly StatusModifiersList Modifiers;
    
    // --- TIME ---
    private float _elapsedTime;
    private float _tickTimer;
    public float TimeLeft => Data.duration - _elapsedTime;
    public bool Expired => _elapsedTime >= Data.duration;
    public bool TickedThisFrame { get; private set; }

    // --- CONSTRUCTOR ---

    public StatusEffect(DataStatusEffect data = null, float potency = 1) {
        IsEmpty = !data;
        Data = data;
        Potency = potency;
        TickedThisFrame = false;
        _elapsedTime = 0;
        _tickTimer = EffectTickInterval;

        Modifiers = IsEmpty ? default : new StatusModifiersList(Data, potency);
    }

    // --- UPDATE METHODS ---
    
    public void UpdateCustom(float deltaTime) { }
    
    public void FixedUpdateCustom(float deltaTime, int tick) {
        TickedThisFrame = false;
        _elapsedTime += deltaTime;
        _tickTimer += deltaTime;

        if (_tickTimer >= EffectTickInterval) {
            TickedThisFrame = true;
            _tickTimer -= EffectTickInterval;
        }
    }

    // --- HELPER METHODS ---

    /// Refreshes duration. Optionally sets effect's current duration to given value if newDuration is above 0. 
    public void Refresh(float newDuration = -1) => _elapsedTime = newDuration >= 0 ? Data.duration - newDuration : 0;
    /// Returns whether effect's name matches given name.
    public bool CompareName(string desiredName) => Name.Equals(desiredName, StringComparison.OrdinalIgnoreCase);
    /// Returns whether given effect is the same as this one via name comparison.
    public bool Compare(DataStatusEffect other) => CompareName(other.name);
}
