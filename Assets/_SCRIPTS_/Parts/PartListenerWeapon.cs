using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartListenerWeapon : Part {
    // --- WEAPON REFERENCES ---
    protected WeaponBase Weapon;
    
    // --- EVENTS ---
    [Header("Weapon Events")]
    public UnityEvent<Health[]> EventCompsHit;
    public UnityEvent<Vector3> EventTap;
    public UnityEvent<Vector3[]> EventSwipe;
    public UnityEvent EventShake;
    
    // ------ START FUNCTIONS ------
    
    protected override void OnStart() {
        if (!transform.parent || !transform.parent.TryGetComponent(out Weapon)) {
            Debug.LogError("Could not find weapon on parent.");
            return;
        }
        
        Weapon.EventOnHit += EventCompsHit.Invoke;
        Weapon.EventOnTap += EventTap.Invoke;
        Weapon.EventOnSwipe += EventSwipe.Invoke;
        Weapon.EventOnShake += EventShake.Invoke;
    }
    
    // ------ PART METHODS ------

    protected override void InvokeLogic() { }
    public override void Reset() { }
}