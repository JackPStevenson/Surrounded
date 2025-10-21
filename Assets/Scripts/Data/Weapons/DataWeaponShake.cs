using UnityEngine;


[CreateAssetMenu(fileName = "Data Weapon Shake", menuName = "Data/Weapon/Shake")]
public class DataWeaponShake : DataWeapon {
    public DataWeaponShake() {
        displayName = "Shake Weapon";
        description = "A shake weapon.";
        
        range = -1;
        penetration = -1;
    }
}