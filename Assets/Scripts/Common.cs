using UnityEngine;

public delegate void GameStateIntDelegate(GameState gameState, int intVal);
public delegate void GenericDelegate();
public delegate void IntDelegate(int value);
public delegate void BoolDelegate(bool value);
public delegate void IntBoolDelegate(int intVal, bool boolVal);
public delegate void IntIntFloatDelegate(int int1Val, int int2Val, float floatVal);
public delegate void Vector2Delegate(Vector2 input);

public enum GameState {
    Intermission,
    InProgress,
    Dead
}

