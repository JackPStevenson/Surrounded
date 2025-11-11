using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicHitComps : Part {
    // --- TRACKING ---
    [Header("Damageable Tracking")]
    public int minHitsForActivation = 1;
    private int _latestHits = 0;

    // ------ PART FUNCTIONS ------
    
    public void SetHitCount(Health[] comps) {
        _latestHits = comps.Length;
        Activated = _latestHits >= minHitsForActivation;
    }
    protected override void InvokeLogic() { }
    public override void Reset() {
        Activated = false;
        _latestHits = 0;
    }
}