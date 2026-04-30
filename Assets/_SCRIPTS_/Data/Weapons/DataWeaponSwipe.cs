using UnityEngine;

[CreateAssetMenu(fileName = "D_Weapon_Swipe", menuName = "Data/Weapon/Swipe")]
public class DataWeaponSwipe : DataWeapon {
    [Header("Swiping")]
    public float maxPathDistance = 5;

    public DataWeaponSwipe() {
        displayName = "Swipe Weapon";
        description = "A swipe weapon.";
        
        penetration = -1;
    }

    public override int GetTypeId() => 1;
}
