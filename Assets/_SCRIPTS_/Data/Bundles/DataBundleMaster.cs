using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_Master", menuName = "Data/Bundle/Master")]
public class DataBundleMaster : DataBundle {
    [Header("Bundles")]
    public DataBundleWeapons BundleWeapons;
    public DataBundlePerksPlayer BundlePerksPlayer;
    public DataBundleZombies BundleZombies;
}