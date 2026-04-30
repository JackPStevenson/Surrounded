using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_Zombies", menuName = "Data/Bundle/Zombies")]
public class DataBundleZombies : DataBundle {
    [Header("Zombies")]
    public List<DataZombie> Zombies;
    
    public DataZombie GetZombie(string zombieName) => Zombies.FirstOrDefault(x => string.Compare(x.name, zombieName, StringComparison.Ordinal) == 0);
}

