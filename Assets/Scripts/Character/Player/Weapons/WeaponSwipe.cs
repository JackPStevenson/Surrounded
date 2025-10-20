using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSwipe : WeaponBase {
    public event Action<Vector3[]> EventOnSwipe;
    
    // --- DATA REFERENCES ---
    public float MaxPathDistance => Mathf.Max(_weaponDataSwipe.maxPathDistance, WeaponManager.MinSwipeDistance);
    
    [Header("Debug")]
    public LineRenderer debugLine;

    // --- PATH ---
    private List<Vector3> _pathPoints;
    int PointCount => _pathPoints.Count;
    Vector3 LastPoint => PointCount > 0 ? _pathPoints[^1] : Vector3.zero;
    
    private float _currentPathLength;

    // ------ START METHODS ------

    private WeaponDataSwipe _weaponDataSwipe;
    protected override bool TryParseData() {
        if (weaponData.GetType() != typeof(WeaponDataSwipe)) return false;
        _weaponDataSwipe = (WeaponDataSwipe) weaponData;
        return true;
    }
    
    protected override void OnStart() {
        _pathPoints = new List<Vector3>();
        debugLine.positionCount = 0;
    }

    // ------ EVENT METHODS ------

    protected override void SwipeAction(Vector3 pos) {
        // Only continue if weapon is enabled and has not yet attacked during current swipe.
        if (!enabled || AttackUsed) return;
        
        // Only continue if path already exists.
        if (PointCount < 1) {
            // If path doesn't exist, add first point. Reset it just in case any straggling values were left behind.
            ResetPath();
            AddPoint(pos);
            return;
        }
        
        // Only continue if point will be valid on path or if point will complete path.
        bool pathCompletedWithPoint = WillPointFinishPath(pos, out Vector3 adjutedPos);
        if (!WillPointBeValid(adjutedPos) && !pathCompletedWithPoint) return;
        
        // Try to use energy to create a valid path.
        if (PointCount < 2){
            // If weapon has enough energy, add point to make path valid.
            if (TryUseEnergy(EnergyCost)) AddPoint(adjutedPos);
            // If path could not be made valid, cancel weapon's attack for this swipe.
            else {
                ResetPath(true);
                return;
            }
        }
        
        // Add point to path. If path is now complete, perform attack early.
        AddPoint(adjutedPos);
        if (pathCompletedWithPoint) TrySwipeAttack();
    }

    protected override void TouchReleaseAction(Vector3 pos) {
        if(!enabled) return;
        
        // If weapon hasn't yet attacked, do attack.
        if (!AttackUsed) TrySwipeAttack();
    
        // Ensure attack buffer is turned off once swipe concludes.
        AttackUsed = false;
    }

    private void TrySwipeAttack() {
        AttackUsed = true;
        
        // Only proceed if at least 2 swipe points are in array. 
        if (_pathPoints.Count <= 1) {
            ResetPath(true);
            return;
        }

        // Try to find zombies along path and damage to all zombies found.
        Health[] damageables = Common.FindHealthsOnPath(_pathPoints.ToArray(), Range, Penetration, HitMask);
        
        // If any damageables were found, damage them.
        foreach (Health d in damageables) d.ModHealth(Damage);
        OnHit(damageables);
        EventOnSwipe?.Invoke(_pathPoints.ToArray());
        
        ResetPath(true);
    }

    // ------ HELPER METHODS ------

    /// Returns whether given point can be added to path. Can optionally ignore distance check if path is already established (>= 2 path points).
    bool WillPointBeValid(Vector3 point) {
        // True if new point is the first in path.
        if (PointCount == 0) return true;
        // True if new point is far enough away from most recent point added. False if new point is not valid.
        return Vector3.Distance(point, LastPoint) >= WeaponManager.MinSwipeDistance;
    }
    
    // Returns whether point will make path's length reach maximum allowed. Additionally, returns point adjusted to make path not exceed path limit.
    bool WillPointFinishPath(Vector3 point, out Vector3 adjustedPos) {
        adjustedPos = point;
        // Record distance between new point and last path point along with new path distance with new point.
        float distDelta = Vector3.Distance(point, LastPoint);
        float newDistWithPoint = _currentPathLength + distDelta;
        
        // If path distance with new point reaches path limit, adjust point so path's length won't exceed max and return true.
        if (newDistWithPoint < MaxPathDistance) return false;
        adjustedPos = LastPoint + (point - LastPoint).normalized * (newDistWithPoint - MaxPathDistance);
        return true;
    }

    /// Adds a point to path. Returns distance between newly added point and previous one on path.
    void AddPoint(Vector3 point) {
        // Increment path length based on distance from last point on path to new point.
        if (PointCount > 0) _currentPathLength += Vector3.Distance(point, LastPoint);
        _pathPoints.Add(point);

        if (!debugLine) return;
        debugLine.positionCount += 1;
        debugLine.SetPosition(debugLine.positionCount - 1, point);
    }

    /// Resets path and visuals. Optionally expends current attack.
    void ResetPath(bool markAttackUsed = false) {
        if(debugLine) debugLine.positionCount = 0;
        _pathPoints.Clear();
        
        _currentPathLength = 0;

        if (markAttackUsed)
            AttackUsed = true;
    }

    protected override void OnToggleWeapon(bool enabled) {
        ResetPath();
        AttackUsed = false;
    }
}