using System;
using System.Linq;
using UnityEngine;

public class StatusModifiersList {
    private const int ConstSize = (int) AffectorConstType.Count;
    private const int DynamicSize = (int) AffectorConstType.Count;

    private readonly float[] _constMuls, _constAdds, _dynamicAdds;

    public bool HasConstModifiers { get; private set; }
    public bool HasDynamicModifiers { get; private set; }

    // ------ CONSTRUCTORS ------
    
    public StatusModifiersList() : this(null, 1) { }
    public StatusModifiersList(DataStatusEffect effect, float scalar = 1) {
        _constMuls = new float[ConstSize];
        _constAdds = new float[ConstSize];
        _dynamicAdds = new float[DynamicSize];

        HasConstModifiers = false;
        HasDynamicModifiers = false;

        if (effect) UpdateList(effect, scalar);
        else Reset();
    }

    // ------ LIST UPDATING METHODS ------

    public void UpdateList(StatusEffect[] effects, bool resetList = true, bool updateFlags = true) {
        if (resetList) Reset();
        foreach (StatusEffect effect in effects)
            UpdateList(effect.Data, effect.Potency, false, false);
        if (updateFlags) UpdateFlags();
    }

    private void UpdateList(DataStatusEffect effect, float scalar = 1, bool reset = true, bool updateFlags = true) {
        if (reset) Reset();
        foreach (AffectorConstant affector in effect.constantAffectors) {
            if (affector.additive) _constAdds[(int) affector.type] += affector.Get(scalar);
            else _constMuls[(int) affector.type] *= affector.Get(scalar);
        }
        foreach (AffectorDynamic affector in effect.dynamicAffectors)
            _dynamicAdds[(int) affector.type] += affector.Get(scalar);
        if (updateFlags) UpdateFlags();
    }

    public void Reset() {
        HasConstModifiers = false;
        HasDynamicModifiers = false;

        Array.Fill(_constMuls, 1);
        Array.Fill(_constAdds, 0);
        Array.Fill(_dynamicAdds, 0);
    }

    private void UpdateFlags() {
        HasConstModifiers = _constMuls.Any(n => !Mathf.Approximately(n, 1)) || _constAdds.Any(n => !Mathf.Approximately(n, 0));
        HasDynamicModifiers = _dynamicAdds.Any(n => !Mathf.Approximately(n, 0));
    }

    // ------ HELPER METHODS ------

    public float ModifyConstant(AffectorConstType type, float baseValue) => HasConstModifiers ? ((baseValue + _constAdds[(int) type]) * _constMuls[(int) type]) : baseValue;
    public float GetDynamicModifier(AffectorDynamicType type) => HasDynamicModifiers ? _dynamicAdds[(int) type] : 0;
}