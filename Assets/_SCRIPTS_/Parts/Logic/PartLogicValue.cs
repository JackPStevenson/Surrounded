using UnityEngine;
using UnityEngine.Events;

public class PartLogicValue : Part {

    [Header("Identification")]
    public string valueName = CharacterCore.PerkScalarTag;
    public float valueModifier = 1;
    public float valueOffset = 0;
    private float _value;

    // --- EVENTS ---
    public UnityEvent<float> EventValueChanged;
    
    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() { }
    
    public float GetValue() => (_value * valueModifier) + valueOffset;
    
    public void SetValue(float value) {
        _value = value;
        EventValueChanged?.Invoke(GetValue());
    }
    
    public override void Reset() => _value = 0;

    public bool CompareName(string comparison) => string.CompareOrdinal(valueName, comparison) == 0;
}
