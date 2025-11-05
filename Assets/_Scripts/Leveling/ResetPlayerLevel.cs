using UnityEngine;

public class ResetPlayerLevel : MonoBehaviour {
    public void Reset() => PlayerPrefs.SetInt("MaxWaveReached", 15);
}
