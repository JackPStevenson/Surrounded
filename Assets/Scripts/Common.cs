using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public delegate void GenericDelegate();
public delegate void IntDelegate(int value);
public delegate void BoolDelegate(bool value);
public delegate void IntBoolDelegate(int intVal, bool boolVal);
public delegate void FloatFloatDelegate(float float1Val, float float2Val);
public delegate void IntIntFloatDelegate(int int1Val, int int2Val, float floatVal);
public delegate void Vector2Delegate(Vector2 input);
public delegate void Vector3Delegate(Vector3 input);
public delegate void ZombieStateDelegate(ZombieState zombieState);
public delegate void GameStateIntDelegate(GameState gameState, int intVal);

public enum ZombieState {
    Approaching,
    Charging,
    AttackingSideTarget,
    AttackingMainTarget
}

public enum GameState {
    Intermission,
    InProgress,
    Dead
}

public static class Common {
    private const int HitListSize = 512;
    
    
    // ------ TEMP LISTS ------

    readonly static Collider[] HitColListTemp = new Collider[HitListSize]; 
    readonly static Collider[] HitColList = new Collider[HitListSize];
    readonly static Damageable[] HitDamList = new Damageable[HitListSize];
    readonly static Damageable[] EmptyHitDamList = Array.Empty<Damageable>();
    
    private static int queryStamp;

    // ------ DETECTION FUNCTIONS ------

    /// Tries to find all Damageables in radius around given point using given hitMask. No more than maxDamageables damageables will be returned.
    public static Damageable[] FindDamageablesInSphere(Vector3 point, float radius, int maxDamageables, LayerMask hitMask) {
        // Check if any objects were found in range of point. Return empty array if nothing was found.
        int hitCols = Physics.OverlapSphereNonAlloc(point, radius, HitColList, hitMask);
        if (hitCols < 1) return EmptyHitDamList;

        // Look through each collider hit. Stop loop early if hit damageables reaches limit.
        int hitDams = 0;
        for (int i = 0; i < hitCols && hitDams < maxDamageables; i++) {
            // If hit collider's root has damageable component, add it to damageables array.
            Collider c = HitColList[i];
            if(c == null) continue;
            if (!c.transform.root.TryGetComponent(out Damageable d)) continue;

            HitDamList[hitDams] = d;
            hitDams++;
        }

        // Return hit damageables. If none were hit, return empty array.
        if (hitDams == 0) return EmptyHitDamList;

        Damageable[] result = new Damageable[hitDams];
        Array.Copy(HitDamList, result, hitDams);
        return result;
    }

    /// Tries to find all Damageables in radius around path made from given points using given hitMask. No more than maxDamageables damageables will be returned.
    public static Damageable[] FindDamageablesAlongPath(Vector3[] points, float radius, int maxDamageables, LayerMask hitMask) {
        // Handle special cases when path length is < 2.
        switch (points.Length) {
            // If path has 0 points, return empty array.
            case 0: return EmptyHitDamList;
            // If path only has 1 point, simply find damageables in sphere instead.
            case 1: return FindDamageablesInSphere(points[0], radius, maxDamageables, hitMask);
        }

        // Loop through each path segment. Stop loop early if hit damageables reaches limit.
        int hitCols = 0, hitDams = 0;
        for (int i = 0; i < points.Length - 1 && hitDams < maxDamageables; i++) {
            // Check if any colliders were found in range of path segment. Skip to next segment if nothing was found.
            Vector3 p1 = points[i], p2 = points[i + 1];
            int hitColsTemp = Physics.OverlapCapsuleNonAlloc(p1, p2, radius, HitColListTemp, hitMask, QueryTriggerInteraction.Ignore);
            if(hitColsTemp == 0) continue;

            // Loop through each collider found on segment.
            for (int j = 0; j < hitColsTemp; j++) {
                // Add collider to hit array so it can be temporarily disabled to prevent redundant hits.
                Collider c = HitColListTemp[j];
                HitColList[hitCols] = HitColListTemp[j];
                c.enabled = false;
                hitCols++;
                
                // If hit collider's root has damageable component, add it to array.
                if (!c.transform.root.TryGetComponent(out Damageable d)) continue;
                HitDamList[hitDams] = d;
                hitDams++;
            }
        }

        // Reenable hit colliders that were previously disabled during path search.
        for (int i = 0 ; i < hitCols; i++) HitColList[i].enabled = true;

        // Return hit damageables. If none were hit, return empty array.
        if (hitDams == 0) return EmptyHitDamList;

        Damageable[] result = new Damageable[hitDams];
        Array.Copy(HitDamList, result, hitDams);
        return result;
    }
    
    // ------ HELPER FUNCTIONS ------
    
    /// Converts a 3d position to a top-down position.
    public static Vector2 ToTopDownPos(Vector3 pos) => new Vector2(pos.x, pos.z);
    /// Converts a top-down position to a 3d position.
    public static Vector3 To3dPos(Vector2 topDown) => new Vector3(topDown.x, 0, topDown.y);

    // Performs smooth interpolation between from and to independently of framerate.
    public static float SmoothLerp(float from, float to, float remainderAfter1Second, float deltaTime) {
        return ((from - to) * Mathf.Pow(remainderAfter1Second, deltaTime)) + to;
    }

}