using System;

[Serializable]
public class SaveLoadFilePerk {
    public string perkName;
    //public SaveLoadPerkVariance[] perkVariance;

    public SaveLoadFilePerk(string perkName) {
        this.perkName = perkName;
    }
}