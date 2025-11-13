using UnityEngine;


[CreateAssetMenu(fileName = "D_Weapon_Shake", menuName = "Data/Weapon/Shake")]
public class DataWeaponShake : DataWeapon {
    public DataWeaponShake() {
        displayName = "Shake Weapon";
        description = "A shake weapon.";
        
        range = -1;
        penetration = -1;
    }
}