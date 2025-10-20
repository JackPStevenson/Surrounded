using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class WeaponShake : WeaponBase {
    public event Action EventOnShake;
    
    PoorSoul _poorSoul;

    // ------ START FUNCTIONS ------
    
    private WeaponDataShake _weaponDataShake;
    protected override bool TryParseData() {
        if (weaponData.GetType() != typeof(WeaponDataShake)) return false;
        _weaponDataShake = (WeaponDataShake) weaponData;
        return true;
    }
    
    protected override void OnStart() {
        _poorSoul = PoorSoul.Instance;
    }

    // ------ EVENT FUNCTIONS ------

    protected override void ShakeAction() {
        // Only do shake if weapon hasn't attacked yet.
        if (!AttackUsed) TryShakeAction();
        
        // Ensure attack buffer is turned off once swipe concludes.
        AttackUsed = false;
    }

    private void TryShakeAction() {
        AttackUsed = true;
        
        // Try to find damageables within given range of poor soul. If poor soul reference is invalid, use world center instead.
        Vector3 centerPos = _poorSoul ? _poorSoul.Position : Vector3.zero;
        Health[] damageables = Common.FindHealthsInSphere(centerPos, Range, Penetration, HitMask);
        
        // If weapon has enough energy, use it and damage found enemies.
        if(!TryUseEnergy(EnergyCost)) return;
        
        foreach (Health d in damageables) d.ModHealth(Damage);
        OnHit(damageables);
        EventOnShake?.Invoke();
    }
}