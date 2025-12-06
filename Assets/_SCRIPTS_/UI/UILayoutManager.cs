using System;
using UnityEngine;

public class UILayoutManager : MonoBehaviour {
    [Header("References")]
    public Canvas[] canvases;

    [Header("General")]
    public int startingCanvas = 0;
    private int _currentCanvas;

    // ------ START METHODS ------
    
    private void Awake() => SwitchCanvas(startingCanvas);

    // ------ EVENT METHODS ------
    
    public void SwitchCanvas(int canvasIndex) {
        if (canvasIndex == _currentCanvas) return;

        for (int i = 0; i < canvases.Length; i++)
            canvases[i].gameObject.SetActive(i == canvasIndex);
        
        _currentCanvas = canvasIndex;
    }

    public void PlayClickSound() {
        ManagerAudio.PlaySound(SoundType.UI, 0, 1f);
    }

    public void PlayLootBoxOpenedSound() {
        // AudioManager.PlaySoundByIndex(SoundType.UI, 1, 1f);
    }

    public void PlayItemClaimSound() {
        // AudioManager.PlaySoundByIndex(SoundType.UI, 2, 1f);
    }
}
