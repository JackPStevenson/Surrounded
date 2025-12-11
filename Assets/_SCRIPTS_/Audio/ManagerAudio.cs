using UnityEngine;


public class ManagerAudio : MonoSingleton<ManagerAudio>
{
    private AudioSource _audioSource;

    [Header("Debug")]
    public bool playSound;
    public bool printList1;
    public bool printList2;

    // ------ START METHODS ------

    protected override void OnAwake()
    {
        _audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    protected override void OnDestroyed(bool isDeletedInstance) { }

    // ------ UPDATE METHODS ------

    private void FixedUpdate()
    {
        if (playSound)
        {
            playSound = false;
            PlaySoundByIndex(SoundType.Zombie, 0);
        }

        if (printList1)
        {
            printList1 = false;
            DisplayClips(SoundType.Zombie);
        }

        if (printList2)
        {
            printList2 = false;
            DisplayClips(SoundType.UI);
        }
    }

    // ------ SOUND EVENT METHODS ------

    public static void PlaySound(SoundType type, int index, float volume = 1) => Inst.PlaySoundByIndex(type, index, volume);
    private void PlaySoundByIndex(SoundType type, int index, float volume = 1)
    {
        // Modify volume by either Music or Effects volume based on whether sound type is Music.
        float volumeModifier = (type is SoundType.Music) ? ManagerSaveLoad.GetMusicVolume() : ManagerSaveLoad.GetEffectsVolume();
        volume *= volumeModifier;
        if (type == SoundType.Zombie) volume *= 0.4f;

        // Get the group, then the audio, then play it.
        _audioSource.PlayOneShot(ManagerData.GetAudioClip(type, index), volume);
    }

    // ------ HELPER METHODS ------

    public static int GetSoundIndex(SoundType type, string clipName)
    {
        // Get audio clips from corresponding type.
        AudioClip[] clips = ManagerData.GetAudioGroup(type).AudioClips;

        // Try to find and return index of clip with given name. Return -1 otherwise.
        for (int i = 0; i < clips.Length; i++) if (string.CompareOrdinal(clips[i].name, clipName) == 0) return i;
        return -1;
    }

    // ------ DEBUG METHODS ------

    [ContextMenu("Dev/Print All Clip Names")]
    public void PrintAllClips() => ManagerData.DataBundleAudio.PrintAllClipNames();

    public static void DisplayClips(SoundType type) => ManagerData.GetAudioGroup(type).PrintAudioList();
}