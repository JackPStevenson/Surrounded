using UnityEngine;

[CreateAssetMenu(fileName = "Bundle Master", menuName = "Data/Bundle/Master")]
public class DataBundleMaster : DataBundle {
    [Header("Bundles")]
    public DataBundleWeapons BundleWeapons;
    public DataBundlePerksPlayer BundlePerksPlayer;
}