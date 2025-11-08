using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "D_Bundle_Zombies", menuName = "Data/Bundle/Zombies")]
public class DataBundleZombies : ScriptableObject {
    [Header("Zombies")]
    public List<DataZombie> Zombies;
}

