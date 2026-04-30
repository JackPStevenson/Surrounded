using System;
using UnityEngine;

public abstract class DataDisplayable : ScriptableObject, IComparable<DataDisplayable>, IDisplayable {
    [Header("Display Info")]
    public string displayName;
    public string description;
    public string elaborateDescription;
    public Sprite icon;
    public int levelToUnlock;

    public int CompareTo(DataDisplayable other) => levelToUnlock.CompareTo(other.levelToUnlock);
    public string GetDisplayName() => displayName;
    public string GetDescription() => description;
    public string GetElaborateDescription() => elaborateDescription;
    public Sprite GetIcon() => icon;
    public int GetLevelToUnlock() => levelToUnlock;
    public DataDisplayable GetData() => this;
}