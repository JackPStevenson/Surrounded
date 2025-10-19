using UnityEngine;
using UnityEngine.Events;

public class PartWeaponShakeListener : PartWeaponListener {
    // --- WEAPON REFERENCES ---
    private WeaponShake _weaponShake;
    
    // --- WEAPON EVENTS ---
    public UnityEvent onShake;
    
    // ------ START METHODS ------
    
    protected override bool TryParseWeapon() {
        if (Weapon.GetType() != typeof(WeaponShake)) return false;
        _weaponShake = (WeaponShake) Weapon;

        _weaponShake.OnShakeDelegate += onShake.Invoke;
        
        return true;
    }
    
    // ------ EVENT METHODS ------
    
    protected override void InvokeLogic() { }
    public override void Reset() { }
}