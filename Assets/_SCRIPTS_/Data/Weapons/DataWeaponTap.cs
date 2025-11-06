using UnityEngine;

[CreateAssetMenu(fileName = "Data Weapon Tap", menuName = "Data/Weapon/Tap")]
public class DataWeaponTap : DataWeapon {
    public DataWeaponTap() {
        displayName = "Tap Weapon";
        description = "A tap weapon.";
    }
}