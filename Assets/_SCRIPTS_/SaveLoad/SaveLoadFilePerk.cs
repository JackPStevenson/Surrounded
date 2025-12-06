using System;

[Serializable]
public class SaveLoadFilePerk {
    public string perkName;
    public float perkPower;
    //public SaveLoadPerkVariance[] perkVariance;

    public SaveLoadFilePerk(string perkName, float perkPower) {
        this.perkName = perkName;
        this.perkPower = perkPower;
    }
}