using System.Collections;
using UnityEngine;


public class ManagerAudio : MonoSingleton<ManagerAudio>
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _musicAudioSource;

    [SerializeField] private float musicVolumeMod = 0.7f;

    public int currentTrack = 2;

    [Header("Debug")]
    public bool playSound;
    public bool printList1;
    public bool printList2;

    // ------ START METHODS ------

    protected override void OnAwake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        PlaySound(SoundType.Music, currentTrack);
        UpdateMusicVolume();
    }

    protected override void OnDestroyed(bool isDeletedInstance) { }


    // ------ SOUND EVENT METHODS ------

    public static void PlaySound(SoundType type, int index, float volume = 1) => Inst.PlaySoundByIndex(type, index, volume);
    private void PlaySoundByIndex(SoundType type, int index, float volume = 1)
    {
        // Modify volume by either Music or Effects volume based on whether sound type is Music.
        float volumeModifier = (type is SoundType.Music) ? ManagerSaveLoad.GetMusicVolume() : ManagerSaveLoad.GetEffectsVolume();
        volume *= volumeModifier;
        if (type == SoundType.Zombie) volume *= 0.4f;

        // Get the group, then the audio, then play it.
        if (type == SoundType.Music)
        {
            _musicAudioSource.Stop();
            _musicAudioSource.clip = ManagerData.GetAudioClip(type, index);
            _musicAudioSource.Play();
        }
        else
        {
            _audioSource.PlayOneShot(ManagerData.GetAudioClip(type, index), volume);
        }
    }

    public static void SetCurrentTrack(int index)
    {
        ManagerAudio.Inst.currentTrack = index;
        PlaySound(SoundType.Music, ManagerAudio.Inst.currentTrack);
    }

    public static void UpdateMusicVolume()
    {
        ManagerAudio.Inst._musicAudioSource.volume = ManagerSaveLoad.GetMusicVolume() * ManagerAudio.Inst.musicVolumeMod;
    }

    public static void FadeMusicOut() => Inst.FadeMusicOutStart();

    public void FadeMusicOutStart()
    {
        Coroutine coroutine = StartCoroutine(ManagerAudio.Inst.MusicFade());
    }

    public IEnumerator MusicFade()
    {
        for (float i = 0; i < 1; i += 0.1f)
        {
            _musicAudioSource.volume *= 0.8f;
            if (_musicAudioSource.volume < 0.2f)
            {
                _musicAudioSource.volume = 0.2f;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    public static void ToggleMusic()
    {
        AudioSource source = ManagerAudio.Inst._musicAudioSource;
        if (source.isPlaying)
        {
            source.Pause();
        }
        else
        {
            source.UnPause();
        }
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