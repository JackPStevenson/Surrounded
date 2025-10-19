using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct PartCondition {
    public PartBase part;
    public bool inverseCondition;
}

public abstract class PartBase : MonoBehaviour {
    public bool Activated { get; protected set; }
    
    // --- CONDITIONS ---
    public List<PartCondition> partConditions;
    protected bool AllConditionsTrue => partConditions.All(condPart => condPart.part.Activated != condPart.inverseCondition);

    private void Start() => OnStart();
    protected virtual void OnStart(){ }

    // ------ EVENTS ------
    
    public void TryInvoke() {
        if (!AllConditionsTrue) return;

        InvokeLogic();
    }

    protected virtual void InvokeLogic() { }
    public virtual void Reset() { }
}
