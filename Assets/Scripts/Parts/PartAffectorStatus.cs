using System;
using UnityEngine;

public class PartAffectorStatus : PartBase
{
    // --- STATUS ---
    [Header("Effect")]
    public StatusEffectData statusEffectData;

    private Health[] _targetComps = Array.Empty<Health>();

    // ------ HIT UPDATING ------

    protected override void InvokeLogic() {
        foreach (Health comp in _targetComps) {
            if (comp && comp.TryGetComponent(out StatusHandling handler)) {
                handler.AddEffect(statusEffectData);
            }
        }
    }

    public void SetTargetComps(Health[] comps) {
        _targetComps = comps;
    }
}
