using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "D_Status_", menuName = "Data/Status Effect")]
public class DataStatusEffect : ScriptableObject {
    [Header("General")]
    public new string name;
    public float duration;
    public bool stackable;
    
    [Header("Effect")]
    public float potencyScalar = 1;
    public AffectorConstant[] constantAffectors;
    public AffectorDynamic[] dynamicAffectors;
}