using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStat {
    public string Name;
    public int Amount = 0;
    public int Blood = 0;
    public int Exp = 0;

    public PlayerStat(string name) => Name = name;
    public PlayerStat(string name, int amount) { Name = name; Amount = amount; }
    public PlayerStat(string name, int amount, int blood, int exp) { Name = name; Amount = amount; Blood = blood; Exp = exp; }
    
    public void OffsetAmount(int offset) => Amount += offset; 
}

public class PlayerStats {
    public readonly int StartLevel = ManagerSaveLoad.GetLevel(), StartExp = ManagerSaveLoad.GetExperience();
    public string KilledBy {get; private set;} = "";

    public readonly PlayerStat WaveReached = new PlayerStat("Wave Reached");
    public readonly PlayerStat TotalZombieKills = new PlayerStat("Zombie Kills");
    public readonly List<PlayerStat> SpecificZombieKills = new List<PlayerStat>();
    public readonly PlayerStat TotalEarnings = new PlayerStat("Total");

    public int Wave => WaveReached.Amount;
    public int TotalKills => TotalEarnings.Amount;
    public int TotalBlood => TotalEarnings.Blood;
    public int TotalExp => TotalEarnings.Exp;

    // ------ EVENT METHODS ------

    private PlayerStat TryFindZombieStat(string name) => SpecificZombieKills.FirstOrDefault(x => string.Compare(x.Name, name, StringComparison.Ordinal) == 0);
    
    public void AddZombieKill(DataZombie data) { if (TryFindZombieStat(data.name) is { } stat) stat.OffsetAmount(1); else SpecificZombieKills.Add(new PlayerStat(data.name, 1)); }
    public int GetZombieKills(string name) => (TryFindZombieStat(name) is { } stat) ? stat.Amount : 0;
    
    public void Finalize(int waveReached, string killedBy) {
        WaveReached.Amount = waveReached;
        KilledBy = killedBy;
        
        // Calculate blood and experience from waves.
        int waveCount = Mathf.Max(WaveReached.Amount - 1, 0);
        WaveReached.Blood = ManagerRewards.GetBloodFromWaves(waveCount);
        WaveReached.Exp = ManagerRewards.GetExp(WaveReached.Blood, waveCount);

        // Calculate blood and experience from specific zombie kills.
        foreach (PlayerStat stat in SpecificZombieKills) {
            stat.Blood = ManagerRewards.GetBloodFromZombies(stat.Name, stat.Amount);
            stat.Exp = ManagerRewards.GetExp(stat.Blood, stat.Amount);
            
            // Add amount, blood, and experience from specific zombie kills to total.
            TotalZombieKills.Amount += stat.Amount;
            TotalZombieKills.Blood += stat.Blood;
            TotalZombieKills.Exp += stat.Exp;
        }
        
        // Calculate total earnings from wave reached and total zombie kills.
        TotalEarnings.Blood = WaveReached.Blood + TotalZombieKills.Blood;
        TotalEarnings.Exp = WaveReached.Exp + TotalZombieKills.Exp;
    }
}