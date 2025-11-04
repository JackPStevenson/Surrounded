using System;
using UnityEngine;
using UnityEngine.Serialization;

/// Affectors for modifying base parameters.
public enum AffectorConstType {
    BaseHealth,
    BaseResistance,
    BaseSpeed,
    BaseAttackDamage,
    BaseAttackRange,
    BaseAttackPenetration,
    BaseAttackLength,
    BaseEnergy,
    BaseEnergyRegen,
    
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
    public float influenceFromStatus;
    
    public AffectorConstant(AffectorConstType type, bool additive = false, float baseModifier = 1, float influenceFromStatus = 1) {
        this.type = type;
        this.additive = additive;
        this.baseModifier = baseModifier;
        this.influenceFromStatus = influenceFromStatus;
    }
    
    public float GetModifier(float scalar = 1) {
        if (additive) return Mathf.Lerp(baseModifier, baseModifier * scalar, influenceFromStatus);
        return 1 + Mathf.Lerp((baseModifier - 1), (baseModifier - 1) * scalar, influenceFromStatus);
    }
}

[Serializable]
public struct AffectorDynamic {
    [Header("Type")]
    public AffectorDynamicType type;
    
    [Header("Potency")]
    public float basePotency;
    public float influenceFromStatus;

    public AffectorDynamic(AffectorDynamicType type, float basePotency = 1, float influenceFromStatus = 1) {
        this.type = type;
        this.basePotency = basePotency;
        this.influenceFromStatus = influenceFromStatus;
    }
    
    public float GetPotency(float scalar = 1) => Mathf.Lerp(basePotency, basePotency * scalar, influenceFromStatus);
}


