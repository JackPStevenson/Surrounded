using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PartAffectorDamage : PartAffector {
    // --- DAMAGE ---
    [Header("Damage")]
    public float damage;
    public bool oneHitPerComp;
    public bool reportBackToPlayer;
    readonly List<Health> _hitCompTracker = new List<Health>();
    
    // --- EVENTS ---
    public UnityEvent<int> EventDamageApplied = new UnityEvent<int>();
    
    // ------ PART FUNCTIONS ------
    
    protected override void OnCompAffect(Health comp) {
        if (oneHitPerComp) {
            if (_hitCompTracker.Any(c => c == comp)) return;
            _hitCompTracker.Add(comp);
        }
        float remainingHp = comp.DealDamage(reportBackToPlayer ? PlayerCore.Inst.Status.ModConst(AffectorConstType.Damage, damage) : damage);
        EventDamageApplied?.Invoke(_hitCompTracker.Count);
        if (!reportBackToPlayer) return;
        ManagerWeapon.Inst.ForceEventOnHit(new Health[] { comp });
        if(remainingHp <= 0) ManagerWeapon.Inst.ForceEventOnKill(new Health[] { comp });
    }
    
    public override void Reset() {
        base.Reset();
        _hitCompTracker.Clear();
    }
}