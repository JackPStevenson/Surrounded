using UnityEngine;
using UnityEngine.Events;

public class PartWeaponTapListener : PartWeaponListener {
    // --- WEAPON REFERENCES ---
    private WeaponTap _weaponTap;
    
    // --- WEAPON EVENTS ---
    public UnityEvent<Vector3> onTap;
    
    // ------ START METHODS ------
    
    protected override bool TryParseWeapon() {
        if (Weapon.GetType() != typeof(WeaponTap)) return false;
        _weaponTap = (WeaponTap) Weapon;
        
        _weaponTap.OnTapDelegate += onTap.Invoke;
        
        return true;
    }
    
    // ------ EVENT METHODS ------
    
    protected override void InvokeLogic() { }
    public override void Reset() { }
}