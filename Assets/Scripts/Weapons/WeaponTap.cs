using UnityEngine;

public class WeaponTap : WeaponBase {
    // ------ UPDATE METHODS ------

    protected override void OnFixedUpdate(float deltaTime) {
        
    }

    // ------ EVENT METHODS ------
    
    protected override void TouchPressAction(Vector3 pos) {
        // Only proceed if any zombies are in range of tap.
        ZombieBase[] hitZombies = FindZombiesInSphere(pos, range, penetration, _hitMask);
        if (hitZombies == null) return;
        
        if (TryUseEnergy(energyCost)) {
            foreach (ZombieBase t in hitZombies) {
                t.DealDamage(damage);
            }
        }
    }
}