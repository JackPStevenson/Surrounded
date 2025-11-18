using System;

public enum GraphicsSetting { Low, Medium, High }

[Serializable]
public class SaveLoadSettings {
    public int musicVolume;
    public int effectsVolume;
    public GraphicsSetting graphics;
    public bool vibration;

    // ------ CONSTRUCTORS ------

    public SaveLoadSettings(int music = 1, int effects = 1, GraphicsSetting graphics = GraphicsSetting.High, bool vibration = true) {
        musicVolume = music;
        effectsVolume = effects;
        this.graphics = graphics;
        this.vibration = vibration;
    }
}