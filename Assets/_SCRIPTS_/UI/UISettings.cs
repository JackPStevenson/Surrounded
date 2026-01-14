using UnityEngine;
using UnityEngine.UI;

public class UISettings : MonoBehaviour
{
    [Header("Sliders")]
    public Slider effectsSlider;
    public Slider musicSlider;

    // ------ START METHODS ------

    void OnEnable()
    {
        effectsSlider.SetValueWithoutNotify(ManagerSaveLoad.GetEffectsVolume());
        musicSlider.SetValueWithoutNotify(ManagerSaveLoad.GetMusicVolume());
    }

    // ------ EVENT METHODS ------

    public void SetEffectsVolume(float volume)
    {
        ManagerSaveLoad.SetEffectsVolume(volume);
        ManagerSaveLoad.ForceSave();
    }

    public void SetMusicVolume(float volume)
    {
        ManagerSaveLoad.SetMusicVolume(volume);
        ManagerSaveLoad.ForceSave();
        ManagerAudio.UpdateMusicVolume();
    }
}
