using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSwipe : WeaponBase {
    private const float MinSwipeDistance = 0.75f;

    [Header("Swiping")]
    public float maxPathDistance = 5;

    [Header("Debug")]
    public LineRenderer debugLine;

    private bool _usedAttackThisSwipe = false;

    private List<Vector3> _pathPoints;
    private float _currentPathDistance;

    // ------ START METHODS ------

    protected override void OnStart() {
        _pathPoints = new List<Vector3>();
        debugLine.positionCount = 0;
    }

    // ------ EVENT METHODS ------

    protected override void SwipeAction(Vector3 pos) {
        // Only continue if weapon has not yet attacked during current swipe.
        if (_usedAttackThisSwipe) return;
        
        // Only continue if path already exists. If it doesn't, try to add first point if weapon has enough energy.
        if (GetPathSize() == 0) {
            if (TryUseEnergy(energyCost)) TryAddPoint(pos, true);
            return;
        }
        
        // Create variables for new point to be added and whether its addition will push path length to max distance.
        Vector3 newPoint = pos;
        bool pathReachedMaxDistance = false;

        // If new point will make path reach max distance, move new point closer to path's last point to ensure path length with new point won't exceed max distance.
        float possiblePathDistance = _currentPathDistance + Vector3.Distance(_pathPoints[^1], newPoint);
        if (possiblePathDistance >= maxPathDistance) {
            newPoint = (_pathPoints[^1] - newPoint).normalized * (possiblePathDistance - maxPathDistance);
            pathReachedMaxDistance = true;
        }
        
        // Attempt to add new point to list. If path reaches max distance with new point, forcefully add it and do attack early.
        TryAddPoint(newPoint, pathReachedMaxDistance);
        if (pathReachedMaxDistance) TrySwipeAttack();
    }

    protected override void TouchReleaseAction(Vector3 pos) {
        // If weapon hasn't yet attacked, do attack.
        if (!_usedAttackThisSwipe) TrySwipeAttack();
    
        // Ensure attack buffer is turned off once swipe concludes.
        _usedAttackThisSwipe = false;
    }

    private void TrySwipeAttack() {
        // Only proceed if at least 2 swipe points are in array. 
        if (_pathPoints.Count > 1) {
            // Try to find zombies along path and damage to all zombies found.
            Damageable[] hitDamageables = Common.FindDamageablesAlongPath(_pathPoints.ToArray(), range, penetration, hitMask);
            if (hitDamageables != null)
                foreach (Damageable d in hitDamageables)
                    d.DealDamage(damage);
        }

        // Clear path points, reset current path distance, and set attack buffer to true.
        if(debugLine) debugLine.positionCount = 0;
        _pathPoints.Clear();
        
        _currentPathDistance = 0;
        _usedAttackThisSwipe = true;
    }

    // ------ HELPER METHODS ------

    /// Returns amount of points currently on path.
    int GetPathSize() => _pathPoints.Count;

    /// Tries to add given point to path. Returns whether given point can be added to path.
    bool TryAddPoint(Vector3 newPoint, bool forceAddPoint = false) {
        if (!forceAddPoint && !IsNewPointValid(newPoint)) return false;

        _currentPathDistance += AddPoint(newPoint);
        return true;
    }

    /// Returns whether given point can be added to path. Can optionally ignore distance check if path is already established (>= 2 path points).
    bool IsNewPointValid(Vector3 newPoint) {
        // True if new point is the first in path.
        if (_pathPoints.Count == 0) return true;
        // True if new point is far enough away from most recent point added.
        return Vector3.Distance(_pathPoints[^1], newPoint) >= MinSwipeDistance;
        // False if new point is not valid.
    }

    /// Adds a point to path. Returns distance between newly added point and previous one on path.
    float AddPoint(Vector3 newPoint) {
        _pathPoints.Add(newPoint);

        debugLine.positionCount += 1;
        debugLine.SetPosition(debugLine.positionCount - 1, newPoint);

        // Return distance between new point and previous point if path already has a point. Return 0 otherwise.
        return GetPathSize() > 1 ? Vector3.Distance(_pathPoints[^2], _pathPoints[^1]) : 0;
    }
}