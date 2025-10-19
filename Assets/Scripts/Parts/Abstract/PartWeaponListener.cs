using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public abstract class PartWeaponListener : PartBase {
    // --- WEAPON REFERENCES ---
    protected WeaponBase Weapon;
    // private WeaponBase _weaponSpecific; // Make this match your weapon's actual type.
    
    // --- WEAPON EVENTS ---
    [Header("Weapon Events")]
    public UnityEvent<Damageable[]> onDamageablesHit;
    
    // ------ START METHODS ------
    
    protected abstract bool TryParseWeapon();
    
    protected override void OnStart() {
        if (!transform.parent || !transform.parent.TryGetComponent(out Weapon) || !TryParseWeapon()) {
            Debug.LogError("Could not find weapon on parent.");
            return;
        }
        
        Weapon.OnDamageablesHitDelegate += onDamageablesHit.Invoke;
    }
    
    // ------ EVENT METHODS ------

    protected override void InvokeLogic() { }
    public override void Reset() { }
}