using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicLoopThroughComps : Part {
    public UnityEvent<Health> EventIndividualComp;
    public UnityEvent<Vector3> EventIndividualCompPos;
    
    // ------ PART FUNCTIONS ------
    
    public void LoopThroughComps(Health[] comps) {
        foreach (Health comp in comps) {
            EventIndividualComp?.Invoke(comp);
            EventIndividualCompPos?.Invoke(comp.Position);
        }
    }
    
    protected override void InvokeLogic() { }
    public override void Reset() { }
}