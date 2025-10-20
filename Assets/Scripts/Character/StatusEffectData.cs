using UnityEngine;
using UnityEngine.Serialization;

public enum StatusEffectType {
    ModHealthOverTime = 0,
    SpeedModifier = 1,
    DamageModifier = 2,
    ResistanceModifier = 3
}

[CreateAssetMenu(fileName = "StatusEffectData", menuName = "Scriptable Objects/StatusEffectData")]
public class StatusEffectData : ScriptableObject {
    [Header("General")]
    public string effectName;
    public float duration;
    public bool stackable;
    
    [Header("Effect Type")]
    public StatusEffectType type;
    /// Acts as DamageOverTime's damage, SpeedModifier's modifier, etc based on selected type.
    public float value;
}