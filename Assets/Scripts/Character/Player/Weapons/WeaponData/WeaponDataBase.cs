using UnityEngine;

public enum AttackType {
    Tap,
    Swipe,
    Shake
}

public abstract class WeaponDataBase : ScriptableObject {
    [Header("Physics")]
    public LayerMask hitMask;
    
    [Header("General")]
    public float damage = 0.5f;
    public float range = 0.5f;
    public int penetration = 1;

    [Header("Energy")]
    public float energyRegenRate = 1;
    [Range(0, 1)]
    public float energyCost = 0.1f;
}
