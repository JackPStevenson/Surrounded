using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_Master", menuName = "Data/Bundle/Master")]
public class DataBundleMaster : DataBundle {
    [Header("Unlockables")]
    public DataBundleWeapons TapWeapons;
    public DataBundleWeapons SwipeWeapons;
    public DataBundleWeapons ShakeWeapons;
    [Space]
    public DataBundlePerks Perks;
    
    [Header("Zombies")]
    public DataBundleZombies BundleZombies;
}