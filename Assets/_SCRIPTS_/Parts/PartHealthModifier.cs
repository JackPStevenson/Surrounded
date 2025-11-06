using UnityEngine;

public class PartHealthModifier : Part {
    Health _health;
    public int healthMod;
    
    void Start() {
        transform.parent?.TryGetComponent(out _health);
        //if(_health) _health.SetMaxHealth(_health.healthMax + healthMod, true, true);
    }
}
