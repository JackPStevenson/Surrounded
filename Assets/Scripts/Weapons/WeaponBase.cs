using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {
    protected bool _isPressing;
    protected Vector3 _touchPressPos;
    protected Vector3 _swipePos;
    protected Vector3 _touchReleasePos;
    protected Vector3 _swipeStrength;

    [Header("Physics")]
    public LayerMask hitMask;
    
    [Header("General")]
    public float damage = 0.5f;
    public float range = 0.5f;
    public int penetration = 1;

    [Header("Energy")]
    public float energyRegenRate = 1;
    [Range(0, 1)]
    public float energyCost = 1;
    
    protected float _currentEnergy;
    
    // ------ UPDATE FUNCTIONS ------

    void Start() {
        OnStart();
    }
    
    protected virtual void OnStart() {}
    
    // ------ UPDATE FUNCTIONS ------
    
    void FixedUpdate() {
        _currentEnergy = Mathf.Clamp01(_currentEnergy + (Time.fixedDeltaTime * energyRegenRate));
        
        OnFixedUpdate(Time.fixedDeltaTime);
    }

    protected virtual void OnFixedUpdate(float deltaTime) { }
    
    // ------ EVENT FUNCTIONS ------
    
    public void OnTouchPress(Vector3 pos) {
        _isPressing = true;
        _touchPressPos = pos;
        _touchReleasePos = Vector3.zero;
        
        TouchPressAction(pos);
    }
    
    public void OnSwipe(Vector3 pos) {
        SwipeAction(pos);
        _swipePos = pos;
    }
    
    public void OnTouchRelease(Vector3 pos) {
        _isPressing = false;
        _touchReleasePos = pos;
        
        TouchReleaseAction(pos);
    }

    public void OnShake(Vector3 strength) {
        _swipeStrength = strength;

        ShakeAction(strength);
    }

    // ------ ACTION FUNCTIONS ------

    protected virtual void TouchPressAction(Vector3 pos) { }
    protected virtual void SwipeAction(Vector3 pos) { }
    protected virtual void TouchReleaseAction(Vector3 pos) { }
    protected virtual void ShakeAction(Vector3 strength) { }


    
    // ------ HELPER FUNCTIONS ------

    public float GetCurrentEnergy() => _currentEnergy;
    
    /// Returns whether weapon has at least given amount of energy.
    protected bool HasEnoughEnergy(float energyNeeded) => _currentEnergy >= energyNeeded;

    /// Tries to consume given amount of energy from current energy. Returns whether consumption was successful.
    public bool TryUseEnergy(float energyNeeded) {
        if (!HasEnoughEnergy(energyNeeded)) return false;

        ModifyEnergy(-energyNeeded);
        return true;
    }
    
    /// Directly modifies energy value. For most cases, use TryUseEnergy instead.
    public void ModifyEnergy(float value) => _currentEnergy = Mathf.Clamp01(_currentEnergy + value);
    

}