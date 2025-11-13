using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "D_Bundle_Weapons", menuName = "Data/Bundle/Weapons")]
public class DataBundleWeapons : DataBundle {
    [Header("Weapons")]
    public List<DataWeapon> Weapons;

    [Header("Defaults")]
    public int defaultTapIndex = 0;
    public int defaultSwipeIndex = 1;
    public int defaultShakeIndex = 2;
    
    public DataWeaponTap DefaultTap => Weapons[defaultTapIndex] as DataWeaponTap;
    public DataWeaponSwipe DefaultSwipe => Weapons[defaultSwipeIndex] as DataWeaponSwipe;
    public DataWeaponShake DefaultShake => Weapons[defaultShakeIndex] as DataWeaponShake;

    public T[] GetItems<T>() where T : DataWeapon => GetItems<T, DataWeapon>(Weapons);
}