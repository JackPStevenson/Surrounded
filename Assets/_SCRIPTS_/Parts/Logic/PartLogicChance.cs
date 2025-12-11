using UnityEngine;

public class PartLogicChance : Part
{
    // --- COUNTING ---
    [Header("Randomness")]
    [Range(0, 1)]
    public float baseActivationChance = 0.5f;
    
    public new bool Activated => AllConditionsTrue && Random.value < (baseActivationChance + _bonusActivationChance);
    private float _bonusActivationChance; 
    
    public void SetBonusChance(float bonus) => _bonusActivationChance = bonus;
    protected override void InvokeLogic() { }
    public override void Reset() => _bonusActivationChance = 0;
}
