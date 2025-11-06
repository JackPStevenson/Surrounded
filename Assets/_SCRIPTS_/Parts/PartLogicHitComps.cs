using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicHitComps : Part {
    // --- DAMAGEABLE TRACKING ---
    [Header("Damageable Tracking")]
    public int minHitsForActivation = 1;
    private int _latestHits = 0;

    // ------ HIT UPDATING ------
    
    public void SetHitCount(Health[] comps) {
        _latestHits = comps.Length;
        Activated = _latestHits >= minHitsForActivation;
    }
}