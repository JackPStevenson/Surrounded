using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bundle Perks Player", menuName = "Data/Bundle/Perks/Player")]
public class DataBundlePerksPlayer : DataBundle {
    [Header("Perks")]
    public List<DataPerkPlayer> PlayerPerks;
}
