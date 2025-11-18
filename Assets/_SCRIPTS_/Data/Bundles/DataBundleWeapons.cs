using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "D_Bundle_Weapons", menuName = "Data/Bundle/Weapons")]
public class DataBundleWeapons : DataBundle {
    [Header("Weapons")]
    public List<DataWeapon> weapons;
    
    public DataWeapon First() => weapons[0];
    public T First<T>() where T : DataWeapon => GetFirst<T, DataWeapon>(weapons);
    public T[] GetWeapons<T>() where T : DataWeapon => GetItems<T, DataWeapon>(weapons);

    
    public DataWeaponTap FirstTap => Taps[0];
    public DataWeaponTap[] Taps => GetWeapons<DataWeaponTap>();
    
    public DataWeaponSwipe FirstSwipe => Swipes[0];
    public DataWeaponSwipe[] Swipes => GetWeapons<DataWeaponSwipe>();
    
    public DataWeaponShake FirstShake => Shakes[0];
    public DataWeaponShake[] Shakes => GetWeapons<DataWeaponShake>();
}