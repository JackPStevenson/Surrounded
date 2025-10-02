using UnityEngine;

public class WeaponTap : WeaponBase {
    public Transform sphere;
    
    
    // ------ UPDATE METHODS ------

    protected override void OnFixedUpdate(float deltaTime) {
        
    }

    // ------ EVENT METHODS ------
    
    protected override void TouchPressAction(Vector3 pos) {
        // Only proceed if any zombies are in range of tap.
        ZombieBase[] hitZombies = Common.FindZombiesInSphere(pos, range, penetration, _hitMask);
        
        sphere.position = pos;
        if (hitZombies == null) return;
        
        if (TryUseEnergy(energyCost)) {
            foreach (ZombieBase t in hitZombies) {
                t.DealDamage(damage);
            }
        }
    }
}