using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct PartCondition {
    public Part part;
    public bool inverseCondition;
}

public abstract class Part : MonoBehaviour {
    public bool Activated { get; protected set; }
    
    // --- CONDITIONS ---
    public List<PartCondition> partConditions;
    protected bool AllConditionsTrue => partConditions.All(condPart => condPart.part.Activated != condPart.inverseCondition);

    private void Start() => OnStart();
    protected virtual void OnStart(){ }

    // ------ PART FUNCTIONS ------
    
    public void TryInvoke() { if (AllConditionsTrue) InvokeLogic(); }
    protected abstract void InvokeLogic();
    public abstract void Reset();
}
