using UnityEngine;

[CreateAssetMenu(fileName = "AudioIndex", menuName = "Audio SO/AudioIndex")]
public class AudioIndex : ScriptableObject {
    [Header("Audio Groups")]
    public AudioGroup ZombieGroup;
    public AudioGroup PoorSoulGroup;
    public AudioGroup TapGroup;
    public AudioGroup SwipeGroup;
    public AudioGroup ShakeGroup;
    public AudioGroup PerkGroup;
    public AudioGroup UIGroup;
    public AudioGroup MusicGroup;

    // ------ SOUND GETTER METHODS ------
    
    public AudioClip GetZombieSound(int index) => ZombieGroup.GetClip(index);
    public AudioClip GetPoorSoulSound(int index) => PoorSoulGroup.GetClip(index);
    public AudioClip GetTapSound(int index) => TapGroup.GetClip(index);
    public AudioClip GetSwipeSound(int index) => SwipeGroup.GetClip(index);
    public AudioClip GetShakeSound(int index) => ShakeGroup.GetClip(index);
    public AudioClip GetPerkSound(int index) => PerkGroup.GetClip(index);
    public AudioClip GetUISound(int index) => UIGroup.GetClip(index);
    public AudioClip GetMusicSound(int index) => MusicGroup.GetClip(index);

    // ------ DEBUG METHODS ------
    
    public void PrintAllClipNames() {
        Debug.Log("Zombie Clips");
        ZombieGroup.PrintAudioList();
        Debug.Log("Poor Soul Clips");
        PoorSoulGroup.PrintAudioList();
        Debug.Log("Tap Clips");
        TapGroup.PrintAudioList();
        Debug.Log("Swipe Clips");
        SwipeGroup.PrintAudioList();
        Debug.Log("Shake Clips");
        ShakeGroup.PrintAudioList();
        Debug.Log("Perk Clips");
        PerkGroup.PrintAudioList();
        Debug.Log("UI Clips");
        UIGroup.PrintAudioList();
        Debug.Log("Music Clips");
        MusicGroup.PrintAudioList();
    }
}
