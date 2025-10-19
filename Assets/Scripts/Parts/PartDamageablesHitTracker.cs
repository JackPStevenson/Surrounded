using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartDamageablesHitTracker : PartBase {
    // --- DAMAGEABLE TRACKING ---
    [Header("Damageable Tracking")]
    public int minHitsForActivation = 1;
    private int _latestHits = 0;

    // ------ HIT UPDATING ------
    
    public void SetDamageableCount(Damageable[] damageables) {
        _latestHits = damageables.Length;
        print(_latestHits);
        Activated = _latestHits >= minHitsForActivation;
    }

}