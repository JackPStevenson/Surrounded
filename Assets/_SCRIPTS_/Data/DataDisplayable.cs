using UnityEngine;
using UnityEngine.Serialization;

public abstract class DataDisplayable : ScriptableObject {
    [Header("Display Info")]
    public string displayName;
    public string description;
    public Sprite icon;
    public int minWaveToUnlock = 0;
}