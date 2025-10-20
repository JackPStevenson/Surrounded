using UnityEngine;

[CreateAssetMenu(fileName = "Swipe Weapon", menuName = "Weapon Data/Swipe Weapon")]
public class WeaponDataSwipe : WeaponDataBase {
    [Header("Swiping")]
    public float maxPathDistance = 5;

    public WeaponDataSwipe() {
        penetration = -1;
    }
}
