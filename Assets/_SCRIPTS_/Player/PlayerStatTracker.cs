using System;
using UnityEngine;




public class PlayerStatTracker : MonoSingleton<PlayerStatTracker> {
    public PlayerStats playerStats;
    public static PlayerStats PlayerStats => Inst.playerStats;
    
    // ------ START METHODS ------

    protected override void OnAwake() { }

    void Start() {
        ManagerZombies.Inst.ZombiePool.EventZombieReturned += OnZombieReturnedToPool;
        PlayerCore.Inst.EventDeath += OnPlayerDeath;
        
        playerStats = new PlayerStats();
    }

    // ------ EVENT METHODS ------
    
    void OnPlayerDeath(CharacterCore obj, string deathSource) {
        playerStats.Finalize(ManagerGame.CurrentWave, deathSource);
        ManagerRewards.GrantPerformanceRewards(playerStats);
        ManagerSaveLoad.ForceSave();
    }
    
    void OnZombieReturnedToPool(DataZombie data) {
        if (!data) {
            Debug.Log("Data was null.");
            return;
        }
        
        playerStats.AddZombieKill(data);
    }
    
    protected override void OnDestroyed(bool isDeletedInstance) {
        playerStats.Finalize(ManagerGame.CurrentWave, "");
        ManagerRewards.GrantPerformanceRewards(playerStats);
    }
}
