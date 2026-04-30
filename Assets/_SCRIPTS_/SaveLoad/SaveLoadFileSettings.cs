using System;
using UnityEngine;

public enum GraphicsSetting { Low, Medium, High }

[Serializable]
public class SaveLoadSettings {
    public float effectsVolume;
    public float musicVolume;

    // ------ CONSTRUCTORS ------

    public SaveLoadSettings(float effects = 1, float music = 1) {
        effectsVolume = effects;
        musicVolume = music;
    }
    
    // ------ EVENT METHODS ------
    
    public void SetEffectsVolume(float volume) => effectsVolume = Mathf.Clamp01(volume);
    public void SetMusicVolume(float volume) => musicVolume = Mathf.Clamp01(volume);
}