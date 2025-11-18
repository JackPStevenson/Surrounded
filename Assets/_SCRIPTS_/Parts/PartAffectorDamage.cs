using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PartAffectorDamage : PartAffector {
    // --- DAMAGE ---
    [Header("Damage")]
    public float damage;
    public bool oneHitPerComp;
    public bool ownedByPlayer;
    readonly List<Health> _hitCompTracker = new List<Health>();
    
    // --- EVENTS ---
    public UnityEvent<int> EventDamageApplied = new UnityEvent<int>();
    
    // ------ PART FUNCTIONS ------
    
    protected override void OnCompAffect(Health comp) {
        if (oneHitPerComp) {
            if (_hitCompTracker.Any(c => c == comp)) return;
            _hitCompTracker.Add(comp);
        }
        comp.DealDamage(ownedByPlayer ? PlayerCore.Instance.Status.ModConst(AffectorConstType.Damage, damage) : damage);
    }
    
    public override void Reset() {
        base.Reset();
        _hitCompTracker.Clear();
    }
}