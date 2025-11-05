using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartListenerWeapon : Part {
    // --- WEAPON REFERENCES ---
    protected WeaponBase Weapon;
    
    // --- WEAPON EVENTS ---
    [Header("Weapon Events")]
    public UnityEvent<Health[]> onCompsHit;
    public UnityEvent<Vector3> onTap;
    public UnityEvent<Vector3[]> onSwipe;
    public UnityEvent onShake;
    
    // ------ START METHODS ------
    
    protected override void OnStart() {
        if (!transform.parent || !transform.parent.TryGetComponent(out Weapon)) {
            Debug.LogError("Could not find weapon on parent.");
            return;
        }
        
        Weapon.EventOnHit += onCompsHit.Invoke;
        Weapon.EventOnTap += onTap.Invoke;
        Weapon.EventOnSwipe += onSwipe.Invoke;
        Weapon.EventOnShake += onShake.Invoke;
    }
    
    // ------ EVENT METHODS ------

    protected override void InvokeLogic() { }
    public override void Reset() { }
}