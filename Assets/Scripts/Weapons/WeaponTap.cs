using UnityEngine;

public class WeaponTap : WeaponBase {
    void Start() {

    }

    void FixedUpdate() {
        _currentEnergy = Mathf.Clamp01(_currentEnergy + (Time.fixedDeltaTime * energyRegenRate));
    }
    
    protected override void TouchPressAction(Vector3 pos) {
        ZombieBase[] hitZombies = FindZombiesInSphere(pos, range, penetration, _hitMask);
        if (hitZombies == null) return;
        
        if (TryUseEnergy(energyCost)) {
            
            for (int i = 0; i < hitZombies.Length; i++) {
                hitZombies[i].DealDamage(damage);
            }
        }
    }
    
    protected override void SwipeAction(Vector3 pos) {
        
    }
    
    protected override void TouchReleaseAction(Vector3 pos) {
        
    }
}