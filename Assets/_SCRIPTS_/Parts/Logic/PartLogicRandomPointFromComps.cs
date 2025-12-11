using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicRandomPointFromComps : Part {
    public UnityEvent<Vector3> EventRandomPoint;
    
    // ------ PART FUNCTIONS ------

    public void SetComps(Health[] comps) => EventRandomPoint?.Invoke(comps.Length > 0 ? (comps[Random.Range(0, comps.Length)].Position) : Vector3.zero);
    
    protected override void InvokeLogic() { }
    public override void Reset() { }
}