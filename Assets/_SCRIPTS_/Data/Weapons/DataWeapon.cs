using UnityEngine;

public abstract class DataWeapon : DataDisplayable
{
    [Header("References")]
    public GameObject prefab;

    [Header("General")]
    public float damage = 0.5f;
    public float range = 0.5f;
    public int penetration = 1;

    [Header("Energy")]
    public float energyRegenRate = 1;
    [Range(0, 1)]
    public float energyCost = 0.1f;

    [Header("Physics")]
    public LayerMask hitMask;

    [Header("Other")]
    public string sound;
    public GameObject particleEffect;
    public abstract int GetTypeId();
    public bool CheckTypeId(int comparisonId) => comparisonId == GetTypeId();
}
