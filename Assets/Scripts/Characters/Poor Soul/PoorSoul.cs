using System;
using UnityEngine;

public class PoorSoul : Damageable {
    public static PoorSoul Instance;
    
    // ------ START METHODS ------
    
    void Awake() {
        Instance = this;
    }

    protected override void OnStart() {

    }
}
