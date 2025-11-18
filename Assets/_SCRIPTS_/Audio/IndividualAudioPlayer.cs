using UnityEngine;

public class IndividualAudioPlayer : MonoBehaviour {
    [SerializeField] protected SoundType type;

    [SerializeField] protected string[] clipNames;
    [SerializeField] protected int[] clipIndexes;

    [SerializeField] protected bool getAudioIndexAtStart = true;

    private void Start() {
        clipIndexes = new int[clipNames.Length];
        if (getAudioIndexAtStart) {
            GetAudioIndexes();
        }
    }

    public virtual void GetAudioIndexes() {
        for (int i = 0; i < clipNames.Length; i++) {
            int temp = AudioManager.instance.FindSoundIndex(type, clipNames[i]);
            //Debug.Log(clipNames[i] + " is at index " + temp);
            clipIndexes[i] = temp;
        }
    }


    public void DoSomething() {
        PlaySound(clipNames[0]);
    }

    public void PlaySound(string soundName) {
        PlaySound(GetIndex(soundName));
    }

    public void PlaySound(int index) {
        if (index < clipNames.Length)
            AudioManager.instance.PlaySoundByIndex(type, clipIndexes[index]);
    }

    private int GetIndex(string soundName) {
        for (int i = 0; i < clipNames.Length; i++) {
            if (clipNames[i] == soundName) {
                return clipIndexes[i];
            }
        }
        return -1;
    }
}