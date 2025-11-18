using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "D_Bundle_Weapons", menuName = "Data/Bundle/Weapons")]
public class DataBundleWeapons : DataBundle {
    
    [Header("Weapons")]
    public List<DataWeapon> weapons;
    
    // ------ FETCH METHODS ------
    
    private T[] GetWeapons<T>() where T : DataWeapon => GetItems<T, DataWeapon>(weapons);
    private T First<T>() where T : DataWeapon => GetFirst<T, DataWeapon>(weapons);
    
    // ------ TAP FETCH METHODS ------
    
    public DataWeaponTap[] Taps => GetWeapons<DataWeaponTap>();
    public DataWeaponTap FirstTap => First<DataWeaponTap>();
    
    // ------ SWIPE FETCH METHODS ------
    
    public DataWeaponSwipe[] Swipes => GetWeapons<DataWeaponSwipe>();
    public DataWeaponSwipe FirstSwipe => First<DataWeaponSwipe>();
    
    // ------ SHAKE FETCH METHODS ------
    
    public DataWeaponShake[] Shakes => GetWeapons<DataWeaponShake>();
    public DataWeaponShake FirstShake => First<DataWeaponShake>();
}