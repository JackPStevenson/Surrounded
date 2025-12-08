using UnityEngine;

public enum SoundType { Zombie, PoorSoul, Tap, Swipe, Shake, Perk, UI, Music }

public class ManagerAudio : MonoSingleton<ManagerAudio> {
    [SerializeField] private AudioIndex audioIndex;
    private AudioSource _audioSource;

    [Header("Debug")]
    public bool playSound;
    public bool printList1;
    public bool printList2;

    // ------ START METHODS ------
    
    protected override void OnAwake() {
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }
    
    protected override void OnDestroyed(bool isDeletedInstance) { }
    
    // ------ UPDATE METHODS ------
    
    private void FixedUpdate() {
        if (playSound) {
            playSound = false;
            PlaySoundByIndex(SoundType.Zombie, 0);
        }
        
        if (printList1) {
            printList1 = false;
            DisplayClips(SoundType.Zombie);
        }
        
        if (printList2) {
            printList2 = false;
            DisplayClips(SoundType.UI);
        }
    }

    // ------ SOUND EVENT METHODS ------
    
    public static void PlaySound(SoundType type, int index, float volume = 1) => Inst.PlaySoundByIndex(type, index, volume);
    private void PlaySoundByIndex(SoundType type, int index, float volume = 1) {
        // Modify volume by either Music or Effects volume based on whether sound type is Music.
        float volumeModifier = (type is SoundType.Music) ? ManagerSaveLoad.GetMusicVolume() : ManagerSaveLoad.GetEffectsVolume();
        volume *= volumeModifier;
        
        // Get the group, then the audio, then play it.
        switch (type) {
            case SoundType.Zombie: _audioSource.PlayOneShot(audioIndex.GetZombieSound(index), volume); break;
            case SoundType.PoorSoul: _audioSource.PlayOneShot(audioIndex.GetPoorSoulSound(index), volume); break;
            case SoundType.Tap: _audioSource.PlayOneShot(audioIndex.GetTapSound(index), volume); break;
            case SoundType.Swipe: _audioSource.PlayOneShot(audioIndex.GetSwipeSound(index), volume); break;
            case SoundType.Shake: _audioSource.PlayOneShot(audioIndex.GetShakeSound(index), volume); break;
            case SoundType.Perk: _audioSource.PlayOneShot(audioIndex.GetPerkSound(index), volume); break;
            case SoundType.UI: _audioSource.PlayOneShot(audioIndex.GetUISound(index), volume); break;
            case SoundType.Music: _audioSource.PlayOneShot(audioIndex.GetMusicSound(index), volume); break;
        }
    }
    
    // ------ HELPER METHODS ------

    public static int GetSoundIndex(SoundType type, string clipName) => Inst.FindSoundIndex(type, clipName);
    private int FindSoundIndex(SoundType type, string clipName) {
        // Get audio clips from corresponding type.
        AudioClip[] clips;
        switch (type) {
            case SoundType.Zombie: clips = audioIndex.ZombieGroup.AudioClips; break;
            case SoundType.PoorSoul: clips = audioIndex.PoorSoulGroup.AudioClips; break;
            case SoundType.Tap: clips = audioIndex.TapGroup.AudioClips; break;
            case SoundType.Swipe: clips = audioIndex.SwipeGroup.AudioClips; break;
            case SoundType.Shake: clips = audioIndex.ShakeGroup.AudioClips; break;
            case SoundType.Perk: clips = audioIndex.PerkGroup.AudioClips; break;
            case SoundType.UI: clips = audioIndex.UIGroup.AudioClips; break;
            case SoundType.Music: clips = audioIndex.MusicGroup.AudioClips; break;
            default: return -1; // Return -1 if type is somehow invalid.
        }

        // Try to find and return index of clip with given name. Return -1 otherwise.
        for (int i = 0; i < clips.Length; i++) if (string.CompareOrdinal(clips[i].name, clipName) == 0) return i;
        return -1;
    }
    
    // ------ DEBUG METHODS ------
    
    [ContextMenu("Dev/Print All Clip Names")]
    public void PrintAllClips() => audioIndex.PrintAllClipNames();
    
    public static void DisplayClips(SoundType soundType) => Inst.DisplayGroupClips(soundType);
    private void DisplayGroupClips(SoundType type) {
        switch (type) {
            case SoundType.Zombie: audioIndex.ZombieGroup.PrintAudioList(); break;
            case SoundType.PoorSoul: audioIndex.PoorSoulGroup.PrintAudioList(); break;
            case SoundType.Tap: audioIndex.TapGroup.PrintAudioList(); break;
            case SoundType.Swipe: audioIndex.SwipeGroup.PrintAudioList(); break;
            case SoundType.Shake: audioIndex.ShakeGroup.PrintAudioList(); break;
            case SoundType.Perk: audioIndex.PerkGroup.PrintAudioList(); break;
            case SoundType.UI: audioIndex.UIGroup.PrintAudioList(); break;
            case SoundType.Music: audioIndex.MusicGroup.PrintAudioList(); break;
        }
    }
}