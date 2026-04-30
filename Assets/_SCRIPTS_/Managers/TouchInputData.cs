using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public enum TouchInputUsed{
    Mouse,
    Touch,
    None
}

public struct TouchInputData {
    public readonly bool wasPressedThisFrame;
    public readonly bool isPressed;
    public readonly Vector2 touchPos;
    public readonly TouchInputUsed touchInputUsed;

    public TouchInputData(bool wasPressedThisFrame, bool isPressed, Vector2 touchPos, TouchInputUsed touchInputUsed) {
        this.wasPressedThisFrame = wasPressedThisFrame;
        this.isPressed = isPressed;
        this.touchPos = touchPos;
        this.touchInputUsed = touchInputUsed;
    }
    
    public static TouchInputData GetGameTouchInput() {
        TouchInputData touchInputData = new TouchInputData(false, false, Vector2.zero, TouchInputUsed.None);
        
        Mouse mouse = Mouse.current;
        if (mouse != null && (mouse.leftButton.isPressed || mouse.leftButton.wasReleasedThisFrame)) {
            touchInputData = new TouchInputData(
                mouse.leftButton.wasPressedThisFrame,
                mouse.leftButton.isPressed,
                mouse.position.ReadValue(),
                TouchInputUsed.Mouse
            );
        }

        TouchControl touch = (Touchscreen.current != null) ? Touchscreen.current.primaryTouch : null;
        if (touch != null && (touch.press.isPressed || touch.press.wasReleasedThisFrame)) {
            touchInputData = new TouchInputData(
                touch.press.wasPressedThisFrame,
                touch.press.isPressed,
                touch.position.ReadValue(),
                TouchInputUsed.Touch
            );
        }

        return touchInputData;
    }
}
