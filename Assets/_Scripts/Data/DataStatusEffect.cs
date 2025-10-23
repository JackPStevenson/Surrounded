using UnityEngine;
using UnityEngine.Serialization;

public enum StatusEffectType {
    ModHealthOverTime = 0,
    SpeedModifier = 1,
    DamageModifier = 2,
    ResistanceModifier = 3
}

[CreateAssetMenu(fileName = "Data Status Effect", menuName = "Data/Status Effect")]
public class DataStatusEffect : ScriptableObject {
    [Header("General")]
    public string effectName;
    public float duration;
    public bool stackable;
    
    [Header("Effect Type")]
    public StatusEffectType type;
    /// Acts as DamageOverTime's damage, SpeedModifier's modifier, etc based on selected type.
    public float value;
}