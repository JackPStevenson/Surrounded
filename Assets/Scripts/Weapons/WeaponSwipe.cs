using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponSwipe : WeaponBase {
    private const float MinSwipeDistance = 0.75f;
    
    [Header("Debug")]
    public LineRenderer debugLine;

    private bool _usedAttackThisSwipe = false;
    
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
        // Stop trying to add points if attack has already been performed.
        if (_usedAttackThisSwipe) return;

        // If weapon doesn't have enough energy to continue swiping, try to add one last point and attack early.
        if (!HasEnoughEnergy() && _swipePoints.Count > 0) {
            if (IsNewPointValid(pos, true)) AddPoint(pos);
            
            TrySwipeAttack();
            return;
        }
        
        // If weapon has enough energy and pos is valid, add it to path.
         if (IsNewPointValid(pos)) {
             if(_swipePoints.Count > 0)
                 print(Vector3.Distance(_swipePoints[^1], pos) + " " + GetCurrentEnergy() );
            AddPoint(pos);
            
            //  Only consume energy if path has >= 1 points to prevent wasting energy on swipe attempts that go nowhere.
            if(_swipePoints.Count > 0)
                TryUseEnergy(energyCost);

            // If addition of this point to path depletes energy, attack early.
            if (!HasEnoughEnergy())
                TrySwipeAttack();
        }
    }

    protected override void TouchReleaseAction(Vector3 pos) {
        // If weapon hasn't yet attacked 
        if (!_usedAttackThisSwipe) {
            if (IsNewPointValid(pos, true)) AddPoint(pos);
            TrySwipeAttack();
        }

        _usedAttackThisSwipe = false;
    }

    private void TrySwipeAttack() {
        // Only proceed if at least 2 swipe points are in array. 
        if (_swipePoints.Count >= 2) {

            // Only proceed if any zombies are in range of tap.
            Damageable[] hitDamageables = Common.FindDamageablesAlongPath(_swipePoints.ToArray(), range, penetration, hitMask);
            if (hitDamageables != null) {

                if (TryUseEnergy(energyCost)) {
                    foreach (Damageable d in hitDamageables) {
                        d.DealDamage(damage);
                    }
                }
            }
        }
        
        debugLine.positionCount = 0;
        _swipePoints.Clear();
        
        _usedAttackThisSwipe = true;
    }

    /// Returns whether given point can be added to path.
    bool IsNewPointValid(Vector3 newPoint, bool willBeLastPoint = false) {
        // (BEGINNING POINT) Return true if new point will be the first in path and isn't going to be last point. This establishes the path's first point.
        if (_swipePoints.Count == 0 && !willBeLastPoint) return true;
        // (MIDDLE POINTS) Return true if new point will be far enough away from last point in path. This ensures points have proper spacing between one another.
        if (Vector3.Distance(_swipePoints[^1], newPoint) >= MinSwipeDistance) return true;
        // (END POINT) Return true if point will be last in path and path has at least 2 other points. This prevents player from simply 'tapping' with swipe.
        if (willBeLastPoint && _swipePoints.Count >= 2) return true;
        // Return false if point is not valid.
        return false;
    }

    /// Adds a point to path.
    void AddPoint(Vector3 newPoint) {
        _swipePoints.Add(newPoint);
        debugLine.positionCount += 1;
        debugLine.SetPosition(debugLine.positionCount - 1, newPoint);
    }
}
