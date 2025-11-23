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
    
    private T[] GetWeapons<T>(bool sorted = false) where T : DataWeapon => GetItems<T, DataWeapon>(weapons, sorted);
    private T First<T>(bool sorted = false) where T : DataWeapon => GetFirst<T, DataWeapon>(weapons, sorted);
    
    // ------ TAP FETCH METHODS ------
    
    public DataWeaponTap[] Taps => GetWeapons<DataWeaponTap>(true);
    public DataWeaponTap[] TapsUnsorted => GetWeapons<DataWeaponTap>();
    public DataWeaponTap FirstTap => First<DataWeaponTap>(true);
    
    // ------ SWIPE FETCH METHODS ------
    
    public DataWeaponSwipe[] Swipes => GetWeapons<DataWeaponSwipe>(true);
    public DataWeaponSwipe[] SwipesUnsorted => GetWeapons<DataWeaponSwipe>();
    public DataWeaponSwipe FirstSwipe => First<DataWeaponSwipe>(true);
    
    // ------ SHAKE FETCH METHODS ------
    
    public DataWeaponShake[] Shakes => GetWeapons<DataWeaponShake>(true);
    public DataWeaponShake[] ShakesUnsorted => GetWeapons<DataWeaponShake>();
    public DataWeaponShake FirstShake => First<DataWeaponShake>(true);
}