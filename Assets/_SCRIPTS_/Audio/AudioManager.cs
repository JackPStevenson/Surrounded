using UnityEngine;

public enum SoundType
{
    Zombie,
    PoorSoul,
    Tap,
    Swipe,
    Shake,
    Perk,
    UI
}

public class AudioManager : MonoBehaviour {
    [SerializeField] private AudioIndex audioIndex;
    public static AudioManager instance;
    private AudioSource audioSource;

    public bool playsound = false;
    public bool printlist = false;
    public bool printlist2 = false;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update() {
        if (playsound) {
            playsound = false;
            PlaySoundByIndex(SoundType.Zombie, 0);
        }
        if (printlist) {
            printlist = false;
            DisplayClips(SoundType.Zombie);
        }
        if (printlist2) {
            printlist2 = false;
            DisplayClips(SoundType.UI);
        }
    }

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }

    public void DisplayClips(SoundType type) {


        switch (type) {
            case SoundType.Zombie:
                instance.audioIndex.ZombieGroup.PrintAudioList();
                break;
            case SoundType.UI:
                instance.audioIndex.UIGroup.PrintAudioList();
                break;
            case SoundType.PoorSoul:
                instance.audioIndex.PoorSoulGroup.PrintAudioList();
                break;
            case SoundType.Tap:
                instance.audioIndex.TapGroup.PrintAudioList();
                break;
            case SoundType.Swipe:
                instance.audioIndex.SwipeGroup.PrintAudioList();
                break;
            case SoundType.Shake:
                instance.audioIndex.ShakeGroup.PrintAudioList();
                break;
            case SoundType.Perk:
                instance.audioIndex.PerkGroup.PrintAudioList();
                break;

            default:
                break;
        }
    }

    public int FindSoundIndex(SoundType type, string name) {

        AudioClip[] clips = null;
        switch (type) {
            case SoundType.Zombie:
                clips = audioIndex.ZombieGroup.AudioClips;
                break;
            case SoundType.UI:
                clips = audioIndex.UIGroup.AudioClips;
                break;
            case SoundType.PoorSoul:
                clips = audioIndex.PoorSoulGroup.AudioClips;
                break;
            case SoundType.Tap:
                clips = audioIndex.TapGroup.AudioClips;
                break;
            case SoundType.Swipe:
                clips = audioIndex.SwipeGroup.AudioClips;
                break;
            case SoundType.Shake:
                clips = audioIndex.ShakeGroup.AudioClips;
                break;
            case SoundType.Perk:
                clips = audioIndex.PerkGroup.AudioClips;
                break;
        }

        if (clips != null) {
            for (int i = 0; i < clips.Length; i++) {
                if (clips[i].name == name) {
                    return i;
                }
            }
        }
        return -1;
    }

    public void PlaySoundByIndex(SoundType type, int index, float volume = 1) {
        // get the group, then the audio, then play it
        
        switch (type) {
            case SoundType.Zombie:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetZombieSound(index), volume);
                break;
            case SoundType.UI:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetUISound(index), volume);
                break;
            case SoundType.PoorSoul:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetPoorSoulSound(index), volume);
                break;
            case SoundType.Tap:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetTapSound(index), volume);
                break;
            case SoundType.Swipe:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetSwipeSound(index), volume);
                break;
            case SoundType.Shake:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetShakeSound(index), volume);
                break;
            case SoundType.Perk:
                instance.audioSource.PlayOneShot(instance.audioIndex.GetPerkSound(index), volume);
                break;

            default:
                break;
        }
    }

    [ContextMenu("Dev/Print All Clip Names")]
    public void PrintAllClips() {
        audioIndex.PrintAllClipNames();
    }
}