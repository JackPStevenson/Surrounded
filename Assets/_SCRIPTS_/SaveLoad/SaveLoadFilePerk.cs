using System;

[Serializable]
public class SaveLoadFilePerk {
    public int perkIndex;
    public float perkPower;
    public DataPerkPlayer Data => ManagerData.GetPerk(perkIndex);

    // ------ CONSTRUCTOR METHODS ------
    
    public SaveLoadFilePerk(int perkIndex, float perkPower = 1) {
        this.perkIndex = perkIndex;
        this.perkPower = perkPower;
    }
}