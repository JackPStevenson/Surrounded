using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_Master", menuName = "Data/Bundle/Master")]
public class DataBundleMaster : DataBundle {
    [Header("Weapons")]
    public DataBundleWeapons weapons;
    [Space]
    public DataWeaponTap defaultTap;
    public DataWeaponSwipe defaultSwipe;
    public DataWeaponShake defaultShake;
    
    [Header("Perks")]
    public DataBundlePerks perks;
    
    [Header("Zombies")]
    public DataBundleZombies zombies;
    
    [Header("Audio")]
    public DataBundleAudio dataBundleAudio;
}