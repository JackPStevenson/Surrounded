using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponTap : WeaponBase {
    public event Action<Vector3> EventOnTap;
    
    [Header("Debug")]
    public Transform debugVisual;

    private Vector3 _tapStartPos;
    
    // ------ START METHODS ------

    private WeaponDataTap _weaponDataTap;
    protected override bool TryParseData() {
        if (weaponData.GetType() != typeof(WeaponDataTap)) return false;
        _weaponDataTap = (WeaponDataTap) weaponData;
        return true;
    }

    protected override void OnStart() {
        _tapStartPos = Vector3.up * -1000;
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
        // Only do tap if weapon hasn't attacked yet.
        if (!AttackUsed) TryTapAction(pos);

        // Ensure attack buffer is turned off once swipe concludes.
        AttackUsed = false;
        
        _tapStartPos = Vector3.up * -1000;
    }

    private void TryTapAction(Vector3 tapPos) {
        AttackUsed = true;
        
        // Check to see if any zombies are in range.
        Health[] damageables = Common.FindHealthsInSphere(tapPos, Range, Penetration, HitMask);

        // If weapon has enough energy, use weapon energy and damage any found damageables.
        if (!TryUseEnergy(EnergyCost)) return;
        foreach (Health d in damageables) d.ModHealth(Damage);
        OnHit(damageables);
        EventOnTap?.Invoke(tapPos);
    }

    // ------ HELPER FUNCTIONS ------
    
    protected override void OnToggleWeapon(bool enabled) {
        _tapStartPos = Vector3.up * -1000;
    }
    
    /// Returns whether given point is too far from starting point.
    bool IsPointTooFarFromStart(Vector3 newPoint) {
        // False if start point hasn't been initialized.
        if (_tapStartPos.y < -100) {
            _tapStartPos = newPoint;
            return false;
        }
        // False if new point is close enough to starting point. True if new point is too far.
        return Vector3.Distance(_tapStartPos, newPoint) < WeaponManager.MinSwipeDistance;
    }
}