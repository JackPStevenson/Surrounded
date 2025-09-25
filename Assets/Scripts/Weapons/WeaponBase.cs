using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {
    protected float _currentEnergy;
    protected bool _isPressing;
    
    protected Vector3 _touchPressPos;
    protected Vector3 _swipePos;
    protected Vector3 _touchReleasePos;

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

    // ------ ABSTRACT FUNCTIONS ------
    
    protected abstract void TouchPressAction(Vector3 pos);
    protected abstract void SwipeAction(Vector3 pos);
    protected abstract void TouchReleaseAction(Vector3 pos);
    
    // ------ HELPER FUNCTIONS ------

    public float GetCurrentEnergy() => _currentEnergy;
    
    /// Tries to consume given amount of energy from current energy. Returns whether consumption was successful
    public bool TryUseEnergy(float energyNeeded) {
        if (!(_currentEnergy >= energyNeeded)) return false;
        _currentEnergy -= energyNeeded;
        return true;
    }
    
    /// Directly modifies energy value. For most cases, use TryUseEnergy instead.
    public void ModifyEnergy(float value) => _currentEnergy = Mathf.Clamp01(_currentEnergy + value);

    private Collider[] _hitObjsTemp;
    /// Tries to find all zombies in radius around given point using given hitMask. No more than maxZombies zombies will be returned.
    protected ZombieBase[] FindZombiesInSphere(Vector3 pos, float radius, int maxZombies, LayerMask hitMask) {
        // Create array with new length if current hit array's length is smaller from required.
        if(_hitObjsTemp.Length < maxZombies) _hitObjsTemp = new Collider[Mathf.Max(maxZombies, 512)];
        
        // Only continue if any colliders were hit.
        int hitObjs = Physics.OverlapSphereNonAlloc(pos, radius, _hitObjsTemp, hitMask);
        if(hitObjs == 0) return null;

        // Look through each collider and add colliders attached to zombies to a list.
        ZombieBase z;
        List<ZombieBase> hitZombies = new List<ZombieBase>();
        for (int i = 0 ; i < hitObjs; i++)
            if (_hitObjsTemp[i].TryGetComponent(out z))
                hitZombies.Add(z);
        
        // Return hit zombies array if any zombies were hit. Otherwise, return null.
        return hitZombies.Count == 0 ? null : hitZombies.ToArray();
    }

}