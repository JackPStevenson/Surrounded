using System;
using UnityEngine;

public class PoorSoul : Health {
    public static PoorSoul Instance;
    
    // ------ START METHODS ------
    
    void Awake() {
        Instance = this;
    }
}
