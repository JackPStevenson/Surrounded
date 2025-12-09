using System;

[Serializable]
public class SaveLoadPerk {
    public int perkIndex;
    public float perkPower;
    public DataPerkPlayer Data => ManagerData.GetPerk(perkIndex);

    // ------ CONSTRUCTOR METHODS ------
    
    public SaveLoadPerk(int perkIndex, float perkPower = 1) {
        this.perkIndex = perkIndex;
        this.perkPower = perkPower;
    }
}