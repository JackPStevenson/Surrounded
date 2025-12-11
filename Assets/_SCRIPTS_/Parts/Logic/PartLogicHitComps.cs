using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicHitComps : Part {
    public UnityEvent<int> EventHitsSet;
    
    // --- TRACKING ---
    [Header("Damageable Tracking")]
    public int minHitsForActivation = 1;
    private int _latestHits = 0;
    
    // ------ PART FUNCTIONS ------
    
    public void SetHitCount(Health[] comps) {
        _latestHits = comps.Length;
        Activated = _latestHits >= minHitsForActivation;
        EventHitsSet?.Invoke(_latestHits);
    }
    protected override void InvokeLogic() { }
    public override void Reset() {
        _latestHits = 0;
        Activated = false;
        EventHitsSet?.Invoke(_latestHits);
    }
}