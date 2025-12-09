using UnityEngine;

public interface IDisplayable {
    public string GetDisplayName();
    public string GetDescription();
    public string GetElaborateDescription();
    public Sprite GetIcon();
    public int GetLevelToUnlock();
}