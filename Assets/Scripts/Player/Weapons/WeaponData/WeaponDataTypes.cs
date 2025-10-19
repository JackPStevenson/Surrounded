using UnityEngine;

public enum AttackType {
    Tap,
    Swipe,
    Shake
}

public abstract class WeaponDataTypes : ScriptableObject {
    [Header("Physics")]
    public LayerMask hitMask;
    public abstract AttackType GetAttackType();
    
    [Header("General")]
    public float damage = 0.5f;
    public float range = 0.5f;
    public int penetration = 1;

    [Header("Energy")]
    public float energyRegenRate = 1;
    [Range(0, 1)]
    public float energyCost = 1;
}

[CreateAssetMenu(fileName = "Tap Weapon", menuName = "Weapon Data/Tap Weapon")]
public class WeaponDataTap : WeaponDataTypes {
    public override AttackType GetAttackType() => AttackType.Tap;
}

[CreateAssetMenu(fileName = "Swipe Weapon", menuName = "Weapon Data/Swipe Weapon")]
public class WeaponDataSwipe : WeaponDataTypes {
    public override AttackType GetAttackType() => AttackType.Swipe;
    
    [Header("Swiping")]
    public float maxPathDistance = 5;
}

[CreateAssetMenu(fileName = "Shake Weapon", menuName = "Weapon Data/Shake Weapon")]
public class WeaponDataShake : WeaponDataTypes {
    public override AttackType GetAttackType() => AttackType.Shake;
}
