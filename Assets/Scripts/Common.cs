using UnityEngine;

public delegate void GenericDelegate();
public delegate void IntDelegate(int value);
public delegate void BoolDelegate(bool value);
public delegate void IntBoolDelegate(int intVal, bool boolVal);
public delegate void FloatFloatDelegate(float float1Val, float float2Val);
public delegate void IntIntFloatDelegate(int int1Val, int int2Val, float floatVal);
public delegate void Vector2Delegate(Vector2 input);
public delegate void Vector3Delegate(Vector3 input);
public delegate void GameStateIntDelegate(GameState gameState, int intVal);

public enum GameState {
    Intermission,
    InProgress,
    Dead
}

public static class Common {
    /// Converts a 3d position to a top-down position.
    public static Vector2 ToTopDownPos(Vector3 pos) => new Vector2(pos.x, pos.z);
    /// Converts a top-down position to a 3d position.
    public static Vector3 To3dPos(Vector2 topDown) => new Vector3(topDown.x, 0, topDown.y);
}

