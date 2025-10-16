using UnityEngine;
using UnityEngine.Serialization;

public class WeaponTap : WeaponBase {
    [Header("Debug")]
    public Transform debugVisual;

    private Vector3 _tapStartPos;
    
    // ------ START METHODS ------

    protected override void OnStart() {
        _tapStartPos = Vector3.up * -1000;
    }
    
    
    // ------ UPDATE METHODS ------

    protected override void OnFixedUpdate(float deltaTime) {
        
    }

    // ------ EVENT METHODS ------

    protected override void SwipeAction(Vector3 pos) {
        // Only continue if attack has not been used yet.
        if (AttackUsed) return;

        // If swipe point strays too far from start point, 
        if (IsPointTooFarFromStart(pos))
            AttackUsed = true;
    }

    protected override void TouchReleaseAction(Vector3 pos) {
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
    
    // ------ HELPER FUNCTIONS ------
    
    protected override void OnToggleWeapon(bool enabled) {
        _tapStartPos = Vector3.up * -1000;
    }
    
    /// Returns whether given point is too far from starting point.
    bool IsPointTooFarFromStart(Vector3 newPoint) {
        // False if start point hasn't been initialized.
        if (_tapStartPos.y < -100) return false;
        // False if new point is close enough to starting point. True if new point is too far.
        return Vector3.Distance(_tapStartPos, newPoint) < WeaponManager.MinSwipeDistance;
    }
}