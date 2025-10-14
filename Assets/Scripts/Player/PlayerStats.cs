using UnityEngine;

public enum ZombieType
{
    Normal,
    Tanky,
    Fast,
    Thrower,
    HighDamage,
    Boss
}

public class ZombieSquashedData
{
    public ZombieType type;
    public int squashedTotal;

    public ZombieSquashedData(ZombieType zType)
    {
        this.type = zType;
        squashedTotal = 0;
    }
}

public class PlayerStats : MonoBehaviour
{
    private int _highestRoundReached = 0;
    private int _zombieBlood = 0;
    private int _zombieBrains = 0;

    // array to store how many zombies of each type the player has squashed
    private ZombieSquashedData[] zombiesSquashedByType = { new ZombieSquashedData(ZombieType.Normal),
        new ZombieSquashedData(ZombieType.Tanky), new ZombieSquashedData(ZombieType.Fast),
        new ZombieSquashedData(ZombieType.Thrower), new ZombieSquashedData(ZombieType.HighDamage),
        new ZombieSquashedData(ZombieType.Boss) };

    // Getter/Setter Methods
    public int TotalZombiesSquashed
    {
        get
        {
            int total = 0;
            foreach (ZombieSquashedData data in zombiesSquashedByType)
            {
                if (data.squashedTotal > 0)
                    total += data.squashedTotal;
            }
            return total;
        }
    }
    public int HighestRoundReached
    {
        get { return _highestRoundReached; }
        set { _highestRoundReached = value; }
    }
    public int ZombieBlood
    {
        get { return _zombieBlood; }
    }
    public int ZombieBrains
    {
        get { return _zombieBrains; }
    }


    // adds an amount to the currently tracked squashed amount for the desired zombie type
    public void IncreaseZombiesSquashed(ZombieType type, uint amt)
    {
        // get the corresponding zombie data and increase the amount squashed
        zombiesSquashedByType[(int)type].squashedTotal += (int)amt;
    }

    // Sets the number of zombies squashed for a specific type of zombie
    public void SetZombiesSquashed(ZombieType type, uint amt)
    {
        // get the corresponding zombie data and increase the amount squashed
        zombiesSquashedByType[(int)type].squashedTotal = (int)amt;
    }

    // Adds zombie blood to the player
    public void AddZombieBlood(uint amt)
    {
        _zombieBlood += (int)amt;
    }

    // Sets the amount of zombie blood the player is currently holding
    public void SetZombieBlood(uint amt)
    {
        _zombieBlood = (int)amt;
    }

    // removes zombie blood from the player. Returns false if they dont have enough zombie blood
    public bool RemoveZombieBlood(uint amt)
    {
        if (_zombieBlood - (int)amt < 0)
        {
            return false;
        }
        else
        {
            _zombieBlood -= (int)amt;
            return true;
        }
    }

    // Adds zombie brains to the player
    public void AddZombieBrains(uint amt)
    {
        _zombieBrains += (int)amt;
    }

    // Sets the amount of zombie brains the player is currently holding
    public void SetZombieBrains(uint amt)
    {
        _zombieBrains = (int)amt;
    }

    // removes zombie brains from the player. Returns false if they dont have enough zombie brains
    public bool RemoveZombieBrains(uint amt)
    {
        if (_zombieBrains - (int)amt < 0)
        {
            return false;
        }
        else
        {
            _zombieBrains -= (int)amt;
            return true;
        }
    }
}
