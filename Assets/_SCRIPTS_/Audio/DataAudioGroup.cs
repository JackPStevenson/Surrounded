using UnityEngine;

[CreateAssetMenu(fileName = "D_AudioGroup", menuName = "Data/AudioGroup")]
public class DataAudioGroup : ScriptableObject {
    [SerializeField] private SoundType type;
    [SerializeField] private AudioClip[] clips;

    public SoundType Type => type;
    public AudioClip[] AudioClips => clips;

    // ------ HELPER METHODS ------

    /// Gets an audio clip at the desired index.
    public AudioClip GetClip(int index) => (index > -1 && index < clips.Length) ? clips[index] : null;

    // ------ DEBUG METHODS ------
    
    /// Prints the audio clip names and corresponding index.
    [ContextMenu("Dev/Print All Clip Names")]
    public void PrintAudioList() {
        string message = "\nindex | name\n";
        for (int i = 0; i < clips.Length; i++)
            message += i + " | " + clips[i].name.ToString() + "\n";
        
        Debug.Log(message);
    }
}