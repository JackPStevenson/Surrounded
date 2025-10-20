using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartHitCompsTracker : PartBase {
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