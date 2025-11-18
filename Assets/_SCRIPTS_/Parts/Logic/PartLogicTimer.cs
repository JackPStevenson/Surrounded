using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicTimer : Part {
    // --- TIMING ---
    [Header("Timer")]
    public float duration = 5;
    public bool repeatTimer = true;
    public bool invokeCanInterrupt;
    private float _currentTimer;
    
    // --- EVENTS ---
    public UnityEvent EventTimer = new UnityEvent();

    // ------ UPDATE FUNCTIONS ------
    
    void FixedUpdate() {
        if (!Activated) return;
        _currentTimer = Mathf.Clamp(_currentTimer - Time.fixedDeltaTime, 0, duration);
        if (_currentTimer > 0) return;
        
        Activated = false;
        if (repeatTimer) TryInvoke();
        EventTimer?.Invoke();
    }

    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() {
        if (Activated && !invokeCanInterrupt) return;
        _currentTimer = duration;
        Activated = true;
    }
    public override void Reset() {
        Activated = false;
        _currentTimer = 0;
    }
}