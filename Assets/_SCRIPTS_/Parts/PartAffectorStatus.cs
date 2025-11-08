using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PartAffectorStatus : PartAffector {
    // --- STATUS ---
    [Header("Effect")]
    public bool effectsPermanent = false;
    public DataStatusEffect[] statusEffects;

    // ------ PART FUNCTIONS ------

    protected override void OnCompAffect(Health comp) {
        if (comp.TryGetComponent(out StatusHandler handler))
            foreach (DataStatusEffect effect in statusEffects)
                handler.TryAddEffect(effect, !effectsPermanent);
    }
}