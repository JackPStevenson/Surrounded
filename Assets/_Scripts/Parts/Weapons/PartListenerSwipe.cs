using UnityEngine;
using UnityEngine.Events;

public class PartListenerSwipe : PartListenerWeapon {
    // --- WEAPON REFERENCES ---
    private WeaponSwipe _weaponSwipe;
    
    // --- WEAPON EVENTS ---
    public UnityEvent<Vector3[]> onSwipe;
    
    // ------ START METHODS ------
    
    protected override bool TryParseWeapon() {
        if (Weapon.GetType() != typeof(WeaponSwipe)) return false;
        _weaponSwipe = (WeaponSwipe) Weapon;

        _weaponSwipe.EventOnSwipe += onSwipe.Invoke;
        
        return true;
    }
    
    // ------ EVENT METHODS ------
    
    protected override void InvokeLogic() { }
    public override void Reset() { }
}