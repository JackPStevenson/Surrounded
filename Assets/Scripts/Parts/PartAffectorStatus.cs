using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PartAffectorStatus : Part
{
    // --- STATUS ---
    [FormerlySerializedAs("statusEffectData")] [Header("Effect")]
    public DataStatusEffect dataStatusEffect;

    private Health[] _targetComps = Array.Empty<Health>();

    // ------ HIT UPDATING ------

    protected override void InvokeLogic() {
        foreach (Health comp in _targetComps) {
            if (comp && comp.TryGetComponent(out StatusHandler handler)) {
                handler.AddEffect(dataStatusEffect);
            }
        }
    }

    public void SetTargetComps(Health[] comps) {
        _targetComps = comps;
    }
}
