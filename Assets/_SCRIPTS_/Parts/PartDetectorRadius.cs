using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartDetectorRadius : Part {
    // --- DETECTION ---
    [Header("Detection")]
    public float radius;
    
    [Header("Continuous Detection")]
    public bool continuousDetection;
    public LayerMask detectionMask;
    public int ticksPerUpdate = 4;
    private Health[] _compsInTrigger = Array.Empty<Health>();
    
    // --- EVENTS ---
    [Header("Detection Events")]
    public UnityEvent<Health[]> onCompsFound;
    
    // --- UPDATE FUNCTIONS ---
    
    private int _tick = 0;
    void FixedUpdate() {
        if (!continuousDetection) return;
        _tick++;
        if (_tick < ticksPerUpdate) return;
        _tick = 0;
        
        TryInvoke();
    }
    
    // --- PART FUNCTIONS ---
    
    protected override void InvokeLogic() {
        _compsInTrigger = Common.FindHealthsInSphere(transform.position, radius, detectionMask);
        Activated = _compsInTrigger.Length > 0;
        if(Activated) onCompsFound?.Invoke(_compsInTrigger);
    }

    public override void Reset() {
        _compsInTrigger = Array.Empty<Health>();
        Activated = false; 
    }
}
