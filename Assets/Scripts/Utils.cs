using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils {
    /// Converts a 3d position to a top-down position.
    public static Vector2 ToTopDownPos(Vector3 pos) => new Vector2(pos.x, pos.z);
    /// Converts a top-down position to a 3d position.
    public static Vector3 To3dPos(Vector2 topDown) => new Vector3(topDown.x, 0, topDown.y);
}