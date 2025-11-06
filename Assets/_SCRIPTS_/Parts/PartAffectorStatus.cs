using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PartAffectorStatus : Part
{
    // --- STATUS ---
    [Header("Effect")]
    public bool effectsPermanent = false;
    public DataStatusEffect[] statusEffects;

    private Health[] _targetComps = Array.Empty<Health>();

    // ------ HIT UPDATING ------

    protected override void InvokeLogic() {
        foreach (Health comp in _targetComps) {
            if (comp && comp.TryGetComponent(out StatusHandler handler)) {
                foreach (DataStatusEffect effect in statusEffects)
                    handler.TryAddEffect(effect, !effectsPermanent);
            }
        }
    }

    public void SetTargetComps(Health[] comps) {
        _targetComps = comps;
    }
}
