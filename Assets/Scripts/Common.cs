using UnityEngine;

public delegate void GameStateDelegate(GameState gameState);
public delegate void GenericDelegate();
public delegate void IntDelegate(int value);
public delegate void BoolDelegate(bool value);
public delegate void IntBoolDelegate(int intVal, bool boolVal);
public delegate void Vector2Delegate(Vector2 input);

public enum GameState {
    Intermission,
    InProgress,
    Dead
}

