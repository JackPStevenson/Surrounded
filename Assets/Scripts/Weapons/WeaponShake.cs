using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class WeaponShake : WeaponBase {
    PoorSoul _poorSoul;

    // ------ START FUNCTIONS ------

    protected override void OnStart() {
        _poorSoul = PoorSoul.Instance;
    }

    // ------ EVENT FUNCTIONS ------

    protected override void ShakeAction() {
        // Only continue if weapon has enough energy.
        if (!HasEnoughEnergy(energyCost)) return;
        
        // Try to find damageables within given range of poor soul. If poor soul reference is invalid, use world center instead.
        Vector3 centerPos = _poorSoul ? _poorSoul.Position : Vector3.zero;
        Damageable[] damageables = Common.FindDamageablesInSphere(centerPos, range, penetration, hitMask);
        //Damageable[] rand = Common.GetRandomDamageablesInList(damageables, penetration, hitMask);
        
        // If any damageables were found, use weapon energy and damage them.
        if(damageables.Length == 0) return;
        TryUseEnergy(energyCost);
        foreach (Damageable d in damageables)
            d.DealDamage(damage);

    }
}