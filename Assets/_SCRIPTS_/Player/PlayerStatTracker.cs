using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatTracker : MonoSingleton<PlayerStatTracker> {
    
    // ------ START METHODS ------

    protected override void OnAwake() { }

    void Start() {
        ManagerZombies.Inst.ZombiePool.EventZombieReturned += OnZombieReturnedToPool;
        PlayerCore.Inst.EventDeath += OnPlayerDeath;
    }

    // ------ EVENT METHODS ------
    
    void OnPlayerDeath(CharacterCore obj, string deathSource) {
        ManagerSaveLoad.AddCareerDeathCause(deathSource);
        ManagerSaveLoad.ForceSave();
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



    protected override void OnDestroyed(bool isDeletedInstance) => ManagerSaveLoad.ForceSave();
}
