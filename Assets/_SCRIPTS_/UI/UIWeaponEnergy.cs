using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(UIDisplayItem))]
public class UIWeaponEnergy : MonoBehaviour {
    public float maxHeight = 300;
    public RectTransform _energyBar;
    [HideInInspector] public UIDisplayItem display;

    private void Awake() {
        TryGetComponent(out display);
    }

    public void SetCharge(float ratio) => _energyBar.anchoredPosition = new Vector2(0, maxHeight * ratio);
    public void ToggleVisual(bool enabled) => display.Icon.color = enabled ? new Color(168, 168, 168) : new Color(100, 100, 100);
}