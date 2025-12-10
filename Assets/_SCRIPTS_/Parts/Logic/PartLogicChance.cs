using UnityEngine;

public class PartLogicChance : Part
{
    // --- COUNTING ---
    [Header("Randomness")]
    [Range(0.0f, 1.0f)]
    public float activationChance = 0.5f;
    
    public new bool Activated => AllConditionsTrue && Random.value < activationChance;
    
    protected override void InvokeLogic() {
        throw new System.NotImplementedException();
    }
    public override void Reset() {
        throw new System.NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
