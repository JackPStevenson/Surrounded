using System;
using UnityEngine;

public enum SoundType { Zombie = 0, PoorSoul = 1, Tap = 2, Swipe = 3, Shake = 4, Perk = 5, UI = 6, Music = 7 }

[CreateAssetMenu(fileName = "D_Bundle_Audio", menuName = "Data/Bundle/Audio")]
public class DataBundleAudio : ScriptableObject {
    [Header("Audio Groups")]
    public DataAudioGroup zombieGroup;
    public DataAudioGroup poorSoulGroup;
    public DataAudioGroup tapGroup;
    public DataAudioGroup swipeGroup;
    public DataAudioGroup shakeGroup;
    public DataAudioGroup perkGroup;
    public DataAudioGroup uiGroup;
    public DataAudioGroup musicGroup;

    // ------ SOUND GETTER METHODS ------

    public AudioClip GetSound(SoundType type, int index) => GetGroup(type).GetClip(index);
    public DataAudioGroup GetGroup(SoundType type) => (int) type switch { 0 => zombieGroup, 1 => poorSoulGroup, 2 => tapGroup, 3 => swipeGroup, 4 => shakeGroup, 5 => perkGroup, 6 => uiGroup, 7 => musicGroup, _ => null };

    // ------ DEBUG METHODS ------
    
    public void PrintAllClipNames() {
        Debug.Log("Zombie Clips");
        zombieGroup.PrintAudioList();
        Debug.Log("Poor Soul Clips");
        poorSoulGroup.PrintAudioList();
        Debug.Log("Tap Clips");
        tapGroup.PrintAudioList();
        Debug.Log("Swipe Clips");
        swipeGroup.PrintAudioList();
        Debug.Log("Shake Clips");
        shakeGroup.PrintAudioList();
        Debug.Log("Perk Clips");
        perkGroup.PrintAudioList();
        Debug.Log("UI Clips");
        uiGroup.PrintAudioList();
        Debug.Log("Music Clips");
        musicGroup.PrintAudioList();
    }
}
