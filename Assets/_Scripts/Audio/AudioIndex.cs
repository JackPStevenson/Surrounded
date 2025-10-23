using UnityEngine;

[CreateAssetMenu(fileName = "AudioIndex", menuName = "Audio SO/AudioIndex")]
public class AudioIndex : ScriptableObject
{
    public AudioGroup PerkGroup;
    public AudioGroup PoorSoulGroup;
    public AudioGroup ShakeGroup;
    public AudioGroup SwipeGroup;
    public AudioGroup TapGroup;
    public AudioGroup UIGroup;
    public AudioGroup ZombieGroup;

    public AudioClip GetPerkSound(int index)
    {
        return PerkGroup.GetClip(index);
    }

    public AudioClip GetPoorSoulSound(int index)
    {
        return PoorSoulGroup.GetClip(index);
    }

    public AudioClip GetShakeSound(int index)
    {
        return ShakeGroup.GetClip(index);
    }

    public AudioClip GetSwipeSound(int index)
    {
        return SwipeGroup.GetClip(index);
    }

    public AudioClip GetTapSound(int index)
    {
        return TapGroup.GetClip(index);
    }

    public AudioClip GetUISound(int index)
    {
        return UIGroup.GetClip(index);
    }

    public AudioClip GetZombieSound(int index)
    {
        return ZombieGroup.GetClip(index);
    }
}
