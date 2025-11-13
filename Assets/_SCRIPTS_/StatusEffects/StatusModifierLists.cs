using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusModifiersList {
    private const int ConstSize = (int) AffectorConstType.Count;
    private const int DynamicSize = (int) AffectorConstType.Count;

    private readonly float[] _constMuls, _constAdds;
    private float[] GetConstArray(AffectorConstant affector) => GetConstArray(affector.additive);
    private float[] GetConstArray(bool additive) => additive ? _constAdds : _constMuls;
    private float GetConst(AffectorConstant affector) => GetConst(affector.type, affector.additive);
    private float GetConst(AffectorConstType type, bool additive = false) => GetConstArray(additive)[(int) type];
    private float GetModifiedConst(AffectorConstant affector, float scalar = 1) => affector.additive ? GetConst(affector) + affector.Get(scalar) : GetConst(affector) * affector.Get(scalar);
    
    private readonly float[] _dynamicAdds;
    private float GetDynamic(AffectorDynamic affector) => _dynamicAdds[(int) affector.type];
    public bool HasDynamicModifiers { get; private set; }

    // ------ CONSTRUCTORS ------

    public StatusModifiersList() : this(null, 1) { }
    public StatusModifiersList(DataStatusEffect effect, float scalar = 1) {
        _constMuls = new float[ConstSize];
        _constAdds = new float[ConstSize];
        _dynamicAdds = new float[DynamicSize];

        if (effect) UpdateList(effect, scalar);
        else Reset();
    }

    // ------ LIST UPDATING METHODS ------
    
    public void UpdateListFromEffects(List<StatusEffect> perm, List<StatusEffect> temp) {
        Reset();
        foreach (StatusEffect effect in perm) UpdateList(effect.Data, effect.Potency, false, false);
        foreach (StatusEffect effect in temp) UpdateList(effect.Data, effect.Potency, false, false);
        UpdateFlags();
    }

    private void UpdateList(DataStatusEffect effect, float scalar = 1, bool reset = true, bool updateFlags = true) {
        if (reset) Reset();
        foreach (AffectorConstant cA in effect.constantAffectors) GetConstArray(cA)[(int) cA.type] = GetModifiedConst(cA, scalar);
        foreach (AffectorDynamic dA in effect.dynamicAffectors) _dynamicAdds[(int) dA.type] += dA.Get(scalar);
        if (updateFlags) UpdateFlags();
    }

    public void Reset() {
        HasDynamicModifiers = false;

        Array.Fill(_constMuls, 1);
        Array.Fill(_constAdds, 0);
        Array.Fill(_dynamicAdds, 0);
    }

    bool IsDefault(float value, bool additive = false) => Mathf.Approximately(value, additive ? 0 : 1);
    private void UpdateFlags() {
        HasDynamicModifiers = false;
        for (int i = 0; i < DynamicSize && !HasDynamicModifiers; i++)
            if (!IsDefault(_dynamicAdds[i], true))
                HasDynamicModifiers = true;
    }

    // ------ HELPER METHODS ------

    public float GetDynamicModifier(AffectorDynamicType type) => HasDynamicModifiers ? _dynamicAdds[(int) type] : 0;
    public float ApplyConstModifier(AffectorConstType type, float baseValue, bool subtract = false, bool clampMin = false) {
        float result = baseValue + GetConst(type, true) * (subtract ? -1 : 1);
        return (clampMin ? Mathf.Max(result, 0) : result) * GetConst(type);
    }
}