using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using Random = UnityEngine.Random;

public enum GameState {
    Intermission,
    InProgress,
    Dead
}

public static class Common {
    private const int HitListSize = 512;
    
    // ------ TEMP LISTS ------

    readonly static Collider[] ColListTemp = new Collider[HitListSize];
    private static int AddColTemp(Collider col, int index) { ColListTemp[index] = col; return index + 1; }
    readonly static Collider[] ColList = new Collider[HitListSize];
    private static int AddCol(Collider col, int index) { ColList[index] = col; return index + 1; }
    readonly static Health[] CompList = new Health[HitListSize];
    private static int AddComp(Health comp, int index) { CompList[index] = comp; return index + 1; }
    readonly static Health[] CompListEmpty = Array.Empty<Health>();

    private static int queryStamp;

    // ------ DETECTION FUNCTIONS ------

    /// Tries to find all health components near given point, with a max of maxHits.
    public static Health[] FindHealthsInSphere(Vector3 point, float radius = -1, int maxHits = -1, int hitMask = -1) {
        // Check if any objects were found in range of point. Return empty array if nothing was found.
        int hitComps = 0, hitCols = Physics.OverlapSphereNonAlloc(point, FallbackFloat(radius, 9999), ColList, FallbackInt(hitMask, Physics.AllLayers));
        
        // Look through each collider hit. Stop loop early if hit components reaches limit.
        for (int i = 0; i < hitCols && hitComps < FallbackInt(maxHits, HitListSize); i++) {
            if (!ColList[i].transform.root.TryGetComponent(out Health comp)) continue;
            hitComps = AddComp(comp, hitComps);
        }

        // Return hit components. If none were hit, return empty component list.
        return FallbackComps(hitComps);
    }

    /// Tries to find all health compoments in radius around path made from given points using given hitMask. No more than maxDamageables damageables will be returned.
    public static Health[] FindHealthsOnPath(Vector3[] points, float radius = -1, int maxHits = -1, int hitMask = -1) {
        // Look through each path segment. Stop loop early if hit components reaches limit.
        int hitComps = 0, hitCols = 0;
        for (int i = 0; i < points.Length - 1 && hitComps < FallbackInt(maxHits, HitListSize); i++) {
            // Check if any colliders were found in range of path segment.
            Vector3 p1 = points[i], p2 = points[i + 1];
            int hitColsTemp = Physics.OverlapCapsuleNonAlloc(p1, p2, FallbackFloat(radius, 9999), ColListTemp, FallbackInt(hitMask, Physics.AllLayers));
            
            // Loop through each collider found on segment.
            for (int j = 0; j < hitColsTemp; j++) {
                // Add collider to hit array so it can be temporarily disabled to prevent redundant hits.
                Collider c = ColListTemp[j];
                c.enabled = false;
                hitCols = AddCol(c, hitCols);

                // If hit collider's root has health component, add it to array.
                if (!c.transform.root.TryGetComponent(out Health comp)) continue;
                hitComps = AddComp(comp, hitComps);
            }
        }

        // Reenable hit colliders that were previously disabled.
        for (int i = 0; i < hitCols; i++) ColList[i].enabled = true;

        // Return hit components. If none were hit, return empty component list.
        return FallbackComps(hitComps);
    }

    // ------ HELPER FUNCTIONS ------

    // Performs smooth interpolation between from and to independently of framerate.
    public static float SmoothLerp(float from, float to, float remainderAfter1Second, float deltaTime) {
        return ((from - to) * Mathf.Pow(remainderAfter1Second, deltaTime)) + to;
    }

    // Performs smooth interpolation between from and to independently of framerate.
    public static Vector3 SmoothLerp(Vector3 from, Vector3 to, float remainderAfter1Second, float deltaTime) {
        return ((from - to) * Mathf.Pow(remainderAfter1Second, deltaTime)) + to;
    }

    // ------ FALLBACK FUNCTIONS ------
    
    private static int FallbackInt(int value, int fallback) => (value > -1) ? value : fallback;
    private static float FallbackFloat(float value, float fallback) => (value > -1) ? value : fallback;
    private static Health[] FallbackComps(int hitComps) {
        if (hitComps == 0) return CompListEmpty;
        Health[] result = new Health[hitComps];
        Array.Copy(CompList, result, hitComps);
        return result;
    }
}