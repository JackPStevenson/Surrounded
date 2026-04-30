using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PartAffectorStatusParent : Part {
    // --- STATUS ---
    [Header("Effect")]
    public bool effectsPermanent = false;
    public DataStatusEffect statusEffect;

    private float _potency = 1;
    
    public void SetPotency(float potency) => _potency = potency;

    // ------ PART FUNCTIONS ------

    protected override void InvokeLogic() {
        if (transform.parent && transform.parent.TryGetComponent(out StatusHandler handler))
            handler.TryAddEffect(statusEffect, !effectsPermanent, _potency);
    }
    
    public override void Reset() { _potency = 1; }
}