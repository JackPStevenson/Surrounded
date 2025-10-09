using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSwipe : WeaponBase {
    [Header("Debug")]
    public LineRenderer debugLine;
    
    private List<Vector3> _swipePoints;
    float lastPointTime = -100;
    
    // ------ START METHODS ------
    
    protected override void OnStart() {
        _swipePoints = new List<Vector3>();
        debugLine.positionCount = 0;
    }
    
    // ------ UPDATE METHODS ------

    protected override void OnFixedUpdate(float deltaTime) {
        
    }

    // ------ EVENT METHODS ------

    protected override void SwipeAction(Vector3 pos) {
        if (_swipePoints.Count < 1 || Vector3.Distance(_swipePoints[^1], pos) > 0.75f) {
            _swipePoints.Add(pos);
            
            debugLine.positionCount += 1;
        }
        
        debugLine.SetPosition(debugLine.positionCount - 1, pos);
    }
    protected override void ShakeAction(Vector3 strength) {
        Debug.Log(strength.magnitude);
    } 

    protected override void TouchReleaseAction(Vector3 pos) {
        if (_swipePoints.Count <= 1) return;
        // Only proceed if any zombies are in range of tap.
        Damageable[] hitDamageables = Common.FindDamageablesAlongPath(_swipePoints.ToArray(), range, penetration, hitMask);
        
        if (hitDamageables == null) return;
        
        if (TryUseEnergy(energyCost)) {
            foreach (Damageable d in hitDamageables) {
                d.DealDamage(damage);
            }
        }

        debugLine.positionCount = 0;
        _swipePoints.Clear();
    }
}
