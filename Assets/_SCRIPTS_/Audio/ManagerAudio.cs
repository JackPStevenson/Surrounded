using UnityEngine;

public enum SoundType {
    Zombie,
    PoorSoul,
    Tap,
    Swipe,
    Shake,
    Perk,
    UI
}

public class ManagerAudio : MonoSingleton<ManagerAudio> {
    [SerializeField] private AudioIndex audioIndex;
    private AudioSource _audioSource;

    [Header("Debug")]
    public bool playSound = false;
    public bool printList1 = false;
    public bool printList2 = false;

    // ------ START METHODS ------
    
    protected override void OnAwake() {
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }
    
    protected override void OnDestroyed(bool isDeletedInstance) { }
    
    // ------ UPDATE METHODS ------
    
    private void Update() {
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
    
    // ------ UPDATE METHODS ------
    
    public static void DisplayClips(SoundType soundType) => Inst.DisplayGroupClips(soundType);
    private void DisplayGroupClips(SoundType type) {
        switch (type) {
            case SoundType.Zombie: audioIndex.ZombieGroup.PrintAudioList(); break;
            case SoundType.UI: audioIndex.UIGroup.PrintAudioList(); break;
            case SoundType.PoorSoul: audioIndex.PoorSoulGroup.PrintAudioList(); break;
            case SoundType.Tap: audioIndex.TapGroup.PrintAudioList(); break;
            case SoundType.Swipe: audioIndex.SwipeGroup.PrintAudioList(); break;
            case SoundType.Shake: audioIndex.ShakeGroup.PrintAudioList(); break;
            case SoundType.Perk: audioIndex.PerkGroup.PrintAudioList(); break;
            default: break;
        }
    }

    public static int GetSoundIndex(SoundType type, string clipName) => Inst.FindSoundIndex(type, clipName);
    private int FindSoundIndex(SoundType type, string clipName) {
        AudioClip[] clips = null;
        switch (type) {
            case SoundType.Zombie: clips = audioIndex.ZombieGroup.AudioClips; break;
            case SoundType.UI: clips = audioIndex.UIGroup.AudioClips; break;
            case SoundType.PoorSoul: clips = audioIndex.PoorSoulGroup.AudioClips; break;
            case SoundType.Tap: clips = audioIndex.TapGroup.AudioClips; break;
            case SoundType.Swipe: clips = audioIndex.SwipeGroup.AudioClips; break;
            case SoundType.Shake: clips = audioIndex.ShakeGroup.AudioClips; break;
            case SoundType.Perk: clips = audioIndex.PerkGroup.AudioClips; break;
        }

        if (clips == null) return -1;
        
        for (int i = 0; i < clips.Length; i++)
            if (clips[i].name == clipName)
                return i;
        
        return -1;
    }

    public static void PlaySound(SoundType type, int index, float volume = 1) => Inst.PlaySoundByIndex(type, index, volume);
    private void PlaySoundByIndex(SoundType type, int index, float volume = 1) {
        // Get the group, then the audio, then play it.
        switch (type) {
            case SoundType.Zombie: _audioSource.PlayOneShot(audioIndex.GetZombieSound(index), volume); break;
            case SoundType.UI: _audioSource.PlayOneShot(audioIndex.GetUISound(index), volume); break;
            case SoundType.PoorSoul: _audioSource.PlayOneShot(audioIndex.GetPoorSoulSound(index), volume); break;
            case SoundType.Tap: _audioSource.PlayOneShot(audioIndex.GetTapSound(index), volume); break;
            case SoundType.Swipe: _audioSource.PlayOneShot(audioIndex.GetSwipeSound(index), volume); break;
            case SoundType.Shake: _audioSource.PlayOneShot(audioIndex.GetShakeSound(index), volume); break;
            case SoundType.Perk: _audioSource.PlayOneShot(audioIndex.GetPerkSound(index), volume); break;
            default: break;
        }
    }

    [ContextMenu("Dev/Print All Clip Names")]
    public void PrintAllClips() {
        audioIndex.PrintAllClipNames();
    }
}