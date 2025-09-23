using System;
using UnityEngine;



public class WaveManager : MonoBehaviour {
    public static WaveManager Instance;
    private ZombieManager _zombieManager;

    private float _waveProgress = 0;
    
    private void Awake() {
        Instance = this;
    }

    void Start() {
        _zombieManager = ZombieManager.Instance;
    }

    public void ToggleSpawning(bool isActive) {
        enabled = isActive;
    }

    void Update() {

    }
}