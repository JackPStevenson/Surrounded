using System;
using UnityEngine;
using UnityEngine.Serialization;

/// Affectors for modifying base parameters.
public enum AffectorConstType {
    MaxHealth,
    Resistance,
    Speed,
    Damage,
    AttackSpeed,
    Range,
    Penetration,
    AttackLength,
    MaxEnergy,
    EnergyRegen,
    
    Count
}

/// Affectors for modifying dynamic variables.
public enum AffectorDynamicType {
    CurrentHealth,
    CurrentEnergy,
    
    Count
}

[Serializable]
public struct AffectorConstant {
    [Header("Type")]
    public AffectorConstType type;
    public bool additive;
    
    [Header("Potency")]
    public float baseModifier;
    public float statusInfluence;
    
    public AffectorConstant(AffectorConstType type, bool additive = false, float baseModifier = 1, float statusInfluence = 1) {
        this.type = type;
        this.additive = additive;
        this.baseModifier = baseModifier;
        this.statusInfluence = statusInfluence;
    }
    
    public float Get(float scalar = 1) => additive ? GetAdd(scalar) : GetMul(scalar);
    private float GetAdd(float scalar) => Mathf.Lerp(baseModifier, baseModifier * scalar, statusInfluence);
    private float GetMul(float scalar) => Mathf.Lerp(baseModifier - 1, (baseModifier - 1) * scalar, statusInfluence) + 1;
}

[Serializable]
public struct AffectorDynamic {
    [Header("Type")]
    public AffectorDynamicType type;
    
    [Header("Potency")]
    public float basePotency;
    [FormerlySerializedAs("influenceFromStatus")] public float statusInfluence;

    public AffectorDynamic(AffectorDynamicType type, float basePotency = 1, float statusInfluence = 1) {
        this.type = type;
        this.basePotency = basePotency;
        this.statusInfluence = statusInfluence;
    }
    
    public float Get(float scalar = 1) => Mathf.Lerp(basePotency, basePotency * scalar, statusInfluence);
}


