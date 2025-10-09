using UnityEngine;

public enum AttackType {
    Tap,
    Swipe
}

[CreateAssetMenu(fileName = "Weapon Data Entry")]
public class WeaponDataEntry : ScriptableObject {
    [Header("General")]
    public AttackType attackType;
    public float damage;
    
    [Header("Energy")]
    public float energyCost;
    public float energyRegenSpeed;
    
    [Header("Attack Shape")]
    public float radius;
}
