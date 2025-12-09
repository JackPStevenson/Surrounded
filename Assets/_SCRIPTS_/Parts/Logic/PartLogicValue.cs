using UnityEngine;
using UnityEngine.Events;

public class PartLogicValue : Part {
    
    [Header("Identification")]
    public string valueName = "Value";
    private float _value;

    // --- EVENTS ---
    public UnityEvent<float> EventValueChanged;
    
    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() { }
    
    public float GetValue() => _value;
    
    public void SetValue(float value) {
        _value = value;
        EventValueChanged?.Invoke(_value);
    }
    
    public override void Reset() => _value = 0;

    public bool Compare(string comparsion) => string.CompareOrdinal(valueName, comparsion) == 0;
}
