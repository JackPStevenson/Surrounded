using UnityEngine;
using UnityEngine.UI;

public class AudioButtonPress : MonoBehaviour {
    [Header("Audio")]
    public int soundIndex = 0;
    private Button _button;
    
    void Awake() => GetComponent<Button>().onClick.AddListener(OnClicked);
    void OnClicked() => ManagerAudio.PlaySound(SoundType.UI, soundIndex);
}
