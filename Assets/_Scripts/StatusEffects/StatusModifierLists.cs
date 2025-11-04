using System;
using System.Linq;
using UnityEngine;

public struct StatusModifiersList {
    private const int ConstSize = (int) AffectorConstType.Count;
    private const int DynamicSize = (int) AffectorConstType.Count;

    private readonly float[] _constMuls, _constAdds, _dynamicAdds;

    public bool HasConstModifiers { get; private set; }
    public bool HasDynamicModifiers { get; private set; }

    // ------ CONSTRUCTORS ------

    public StatusModifiersList(DataStatusEffect effect = null, float scalar = 1) {
        _constMuls = new float[ConstSize];
        _constAdds = new float[ConstSize];
        _dynamicAdds = new float[DynamicSize];

        HasConstModifiers = false;
        HasDynamicModifiers = false;

        if (effect) UpdateList(effect, scalar);
        else {
            ResetList();
            UpdateFlags();
        }
    }

    // ------ LIST UPDATING METHODS ------

    public void UpdateList(StatusEffect[] effects, bool resetList = true, bool updateFlags = true) {
        if (resetList) ResetList();
        foreach (StatusEffect effect in effects)
            if (!effect.IsEmpty)
                UpdateList(effect.Data, effect.Potency, false, false);
        if (updateFlags) UpdateFlags();
    }

    private void UpdateList(DataStatusEffect effect, float scalar = 1, bool resetList = true, bool updateFlags = true) {
        // Set arrays to default values if desired and populate modifier arrays with proper data.
        if (resetList) ResetList();
        foreach (AffectorConstant a in effect.constantAffectors) {
            if (a.additive) _constAdds[Index(a)] += a.GetModifier(scalar);
            else _constMuls[Index(a)] *= a.GetModifier(scalar);
        }
        foreach (AffectorDynamic a in effect.dynamicAffectors) _dynamicAdds[Index(a)] += a.GetPotency(scalar);

        if (updateFlags) UpdateFlags();
    }

    public void ResetList() {
        HasConstModifiers = false;
        HasDynamicModifiers = false;
        for (int i = 0; i < ConstSize; i++) {
            _constMuls[i] = 1;
            _constAdds[i] = 0;
        }
        for (int i = 0; i < DynamicSize; i++) _dynamicAdds[i] = 0;
    }

    private void UpdateFlags() {
        HasConstModifiers = false;
        HasDynamicModifiers = false;
        for (int i = 0; i < ConstSize && !HasConstModifiers; i++)
            if (!Mathf.Approximately(_constMuls[i], 1) || !Mathf.Approximately(_constAdds[i], 0))
                HasConstModifiers = true;
        for (int i = 0; i < DynamicSize && !HasDynamicModifiers; i++)
            if (!Mathf.Approximately(_dynamicAdds[i], 0))
                HasDynamicModifiers = true;
    }

    // ------ HELPER METHODS ------

    private int Index(AffectorConstant affector) => Index(affector.type);
    private int Index(AffectorConstType type) => (int) type;
    private int Index(AffectorDynamic affector) => Index(affector.type);
    private int Index(AffectorDynamicType type) => (int) type;

    public float ModifyGivenConstant(AffectorConstType type, float baseValue) => (_constAdds != null && _constMuls != null) ? (baseValue + _constAdds[Index(type)]) * _constMuls[Index(type)] : baseValue;

    public float GetDynamicModifier(AffectorDynamicType type) => _dynamicAdds != null ? _dynamicAdds[Index(type)] : 0;
}