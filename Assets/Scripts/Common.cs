using System;
using System.Collections.Generic;
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
    private const int HitCollidersSize = 512;
    

    // ------ TEMP LISTS ------
    
    // Make hit lists lazy to reduce memory footprint and to have them only initialize when needed.
    readonly static Lazy<Collider[]> LazyHitColliders = new Lazy<Collider[]>(() => new Collider[HitCollidersSize]);
    readonly static Lazy<List<ZombieBase>> LazyHitZombies = new Lazy<List<ZombieBase>>(() => new List<ZombieBase>());
    static Collider[] HitColliders => LazyHitColliders.Value;
    static List<ZombieBase> HitZombies => LazyHitZombies.Value;


    // ------ DETECTION FUNCTIONS ------
    
    /// Tries to find all zombies in radius around given point using given hitMask. No more than maxZombies zombies will be returned.
    public static ZombieBase[] FindZombiesInSphere(Vector3 pos, float radius, int maxZombies, LayerMask hitMask) {
        // Clear hit lists of any previous hit results.
        Array.Clear(HitColliders, 0, HitCollidersSize);
        HitZombies.Clear();

        int hitObjs = Physics.OverlapSphereNonAlloc(pos, radius, HitColliders, hitMask);
        
        
        // Look through each collider and add any zombies found to a list.
        int colIndex = 0;
        while ((colIndex < hitObjs) && (HitZombies.Count >= maxZombies)) {
            if (HitColliders[colIndex].TryGetComponent(out ZombieBase z))
                HitZombies.Add(z);
            
            colIndex++;
        }

        // Return hit zombies array if any zombies were hit. Otherwise, return null.
        return HitZombies.Count == 0 ? null : HitZombies.ToArray();
    }

    // ------ HELPER FUNCTIONS ------
    /// Converts a 3d position to a top-down position.
    public static Vector2 ToTopDownPos(Vector3 pos) => new Vector2(pos.x, pos.z);
    /// Converts a top-down position to a 3d position.
    public static Vector3 To3dPos(Vector2 topDown) => new Vector3(topDown.x, 0, topDown.y);
}