using UnityEngine;
using UnityEngine.Serialization;

public class WeaponTap : WeaponBase {
    [Header("Debug")]
    public Transform debugVisual;
    
    // ------ UPDATE METHODS ------

    protected override void OnFixedUpdate(float deltaTime) {
        
    }

    // ------ EVENT METHODS ------
    
    protected override void TouchPressAction(Vector3 pos) {
        // Only proceed if any zombies are in range of tap.
        Damageable[] hitDamageables = Common.FindDamageablesInSphere(pos, range, penetration, hitMask);
        
        if (debugVisual) debugVisual.position = pos;
        if (hitDamageables == null) return;
        
        if (TryUseEnergy(energyCost)) {
            foreach (Damageable d in hitDamageables) {
                d.DealDamage(damage);
            }
        }
    }
}