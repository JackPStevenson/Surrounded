using System;
using UnityEngine;

[Serializable]
public class SaveLoadPerk : IDisplayable {
    public int perkIndex;
    public float perkPower;
    public DataPerkPlayer Data => ManagerData.GetPerk(perkIndex);

    // ------ CONSTRUCTOR METHODS ------
    
    public SaveLoadPerk(int perkIndex, float perkPower = 1) {
        this.perkIndex = perkIndex;
        this.perkPower = perkPower;
    }
    
    public string GetDisplayName() => Data.GetDisplayName();
    public string GetDescription() => Data.GetDescription();
    public string GetElaborateDescription() => Data.GetElaborateDescription();
    public Sprite GetIcon() => Data.GetIcon();
    public int GetLevelToUnlock() => Data.GetLevelToUnlock();
    public DataDisplayable GetData() => Data;
}