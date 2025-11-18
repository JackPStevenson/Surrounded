using System;
using UnityEngine;

public class PlayerCore : CharacterCore {
    public static PlayerCore Inst;

    [Header("Vitality")]
    public float maxHealth = 100;
    int _tick = 0;
    
    // ------ START METHODS ------
    
    void Awake() {
        Inst = this;
        Initialize();
        Activate();
    }
    
    // ------ UPDATE METHODS ------
    
    private void Update() => UpdateCustom(Time.deltaTime);
    private void FixedUpdate() {
        FixedUpdateCustom(Time.fixedDeltaTime, _tick);
        _tick++;
    }
    
    // ------ EVENT METHODS ------
    
    protected override void OnActivate() { 
        Health.SetMaxHealth(maxHealth);
    }

    private void OnDestroy() {
        Inst = null;
    }
}
