using System;
using UnityEngine;

public abstract class PartAffector : Part {
    // --- TARGETING ---
    private Health[] _targetComps = Array.Empty<Health>();
    public void SetTargetComps(Health[] comps) => _targetComps = comps;

    // ------ PART FUNCTIONS ------
    
    protected abstract void OnCompAffect(Health comp);
    protected override void InvokeLogic() {
        foreach (Health comp in _targetComps)
            if (comp) OnCompAffect(comp);
    }
    public override void Reset() => _targetComps = Array.Empty<Health>();
}