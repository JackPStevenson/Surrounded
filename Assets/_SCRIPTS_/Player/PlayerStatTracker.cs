using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatTracker : MonoBehaviour {
    public static PlayerStatTracker Instance;
    
    ManagerZombies _zombies;
    PlayerCore _player;
    
    // ------ START METHODS ------

    void Awake() {
        if (Instance) {
            gameObject.SetActive(false);
            Destroy(gameObject);
            return;
        }
        
        Instance = this;
    }

    void Start() {
        _zombies = ManagerZombies.Instance;
        print(_zombies);
        _player = PlayerCore.Instance;
        
        _zombies.ZombiePool.EventZombieReturned += OnZombieReturnedToPool;
        _player.EventDeath += OnPlayerDeath;
    }

    // ------ EVENT METHODS ------
    
    void OnPlayerDeath(CharacterCore obj, string deathSource) {
        ManagerSaveLoad.AddCareerDeathCause(deathSource);
    }
    
    void OnZombieReturnedToPool(DataZombie data) {
        if (!data) {
            Debug.Log("Data was null.");
            return;
        }
        
        ManagerSaveLoad.AddZombieBlood(data.experienceOnDeath);
        ManagerSaveLoad.AddExperience(data.experienceOnDeath);
        ManagerSaveLoad.AddCareerZombieKillCount(data.name);
    }

    void OnDestroy() {
        ManagerSaveLoad.ForceSave();
    }
}
