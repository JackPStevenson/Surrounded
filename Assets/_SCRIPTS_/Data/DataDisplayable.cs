using System;
using UnityEngine;

public abstract class DataDisplayable : ScriptableObject, IComparable<DataDisplayable> {
    [Header("Display Info")]
    public string displayName;
    public string description;
    public string elaborateDescription;
    public Sprite icon;
    public int levelToUnlock;

    public int CompareTo(DataDisplayable other) => levelToUnlock.CompareTo(other.levelToUnlock);
}