using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartListenerWeapon : Part {
    
    // --- EVENTS ---
    [Header("Weapon Events")]
    public UnityEvent<Health[]> EventCompsHit;
    public UnityEvent<Health[]> EventCompsKilled;
    public UnityEvent<Vector3> EventTap;
    public UnityEvent<Vector3[]> EventSwipe;
    public UnityEvent EventShake;
    
    // ------ START FUNCTIONS ------

    protected override void OnStart() {
        if (!transform.parent) Debug.LogError("Attached to object with no parent.");
        else if (transform.parent.TryGetComponent(out WeaponBase weapon)) {
            weapon.EventOnHit += EventCompsHit.Invoke;
            weapon.EventOnKill += EventCompsKilled.Invoke;
            weapon.EventOnTap += EventTap.Invoke;
            weapon.EventOnSwipe += EventSwipe.Invoke;
            weapon.EventOnShake += EventShake.Invoke;
        }
        else if (transform.parent.Find("WeaponManager") is { } manager && manager.TryGetComponent(out ManagerWeapon weaponManager)) {
            weaponManager.EventOnHit += EventCompsHit.Invoke;
            weaponManager.EventOnKill += EventCompsKilled.Invoke;
            weaponManager.EventOnTap += EventTap.Invoke;
            weaponManager.EventOnSwipe += EventSwipe.Invoke;
            weaponManager.EventOnShake += EventShake.Invoke;
        }
        else Debug.LogError("Could not find weapon on parent.");
    }

    // ------ PART METHODS ------

    protected override void InvokeLogic() { }
    public override void Reset() { }
}