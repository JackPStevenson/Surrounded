using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_PerksPlayer", menuName = "Data/Bundle/Player Perks")]
public class DataBundlePerksPlayer : DataBundle {
    [Header("Perks")]
    public List<DataPerkPlayer> PlayerPerks;
}
