using UnityEngine;

[CreateAssetMenu(fileName = "AudioGroup", menuName = "Audio Group")]
public class AudioGroup : ScriptableObject
{
    [SerializeField] private SoundType _type;
    [SerializeField] private AudioClip[] _clips;

    public SoundType Type
    {
        get { return _type; }
    }

    public AudioClip[] AudioClips
    {
        get { return _clips; }
    }

    // gets an audio clip at the desired index 
    public AudioClip GetClip(int index)
    {
        Debug.Log("getting clip at index " + index);
        // if the index is within the bounds of the array
        if (index < _clips.Length)
        {
            // return the clip at [index]
            Debug.Log("returned " + _clips[index].ToString());
            return _clips[index];
        }

        // if the index is outside the bounds of the array return null
        return null;
    }

    // prints the audio clip names and corresponding index
    [ContextMenu("Dev/Print All Clip Names")]
    public void PrintAudioList()
    {
        string msg = "\nindex | name\n";
        for (int i = 0; i < _clips.Length; i++)
        {
            AudioClip clip = _clips[i];
            msg += i + " | " + clip.name.ToString() + "\n";
        }
        Debug.Log(msg);
    }
}