using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_Master", menuName = "Data/Bundle/Master")]
public class DataBundleMaster : DataBundle {
    [Header("Weapons")]
    public DataBundleWeapons Weapons;
    [Space]
    public DataWeaponTap defaultTap;
    public DataWeaponSwipe defaultSwipe;
    public DataWeaponShake defaultShake;
    
    [Header("Perks")]
    public DataBundlePerks Perks;
    
    [Header("Zombies")]
    public DataBundleZombies Zombies;
}