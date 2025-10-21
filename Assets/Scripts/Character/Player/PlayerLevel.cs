using UnityEngine;

public class PlayerLevel : MonoBehaviour
{
    [SerializeField] private LevelingRewardsIndex rewardsIndex;
    public static PlayerLevel instance;

    public uint _level = 0;
    public uint _experience = 0;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // amt of xp that each level will require
    [SerializeField] private int XpRequirementIncrement = 1000;

    // Getter Methods
    public uint CurrentLevel
    {
        get { return _level; }
    }
    public uint Experience
    {
        get { return _experience; }
    }

    // Adds experience to the player
    public void AddExperience(uint amt)
    {
        _experience += amt;
        AttemptLevelUp();
    }

    public void SetExperience(uint amt, bool attemptLevelUp)
    {
        _experience = amt;
        if (attemptLevelUp)
            AttemptLevelUp();
    }

    // levels the player up if they have enough xp
    private void AttemptLevelUp()
    {
        Debug.Log("attempting level up");
        uint xpReq = GetCurrentXPRequirement();
        if (_experience >= xpReq)
        {
            // level up
            LevelUp();
        }
        else
        {
            Debug.Log("level up failed");
        }
    }

    // increases the player level and removes the required amount of experience
    private void LevelUp()
    {
        Debug.Log("level up successful");

        // remove xp from the player
        _experience -= GetCurrentXPRequirement();
        // increment level
        SetLevel(_level + 1, false);
        // give the player a level up reward
        //GetLevelUpReward();
        // attempt to level up again incase they earned enough xp for 2 level ups
        AttemptLevelUp();
    }

    // Sets the player level and if resetXP is true, resets the player's xp to zero
    public void SetLevel(uint level, bool resetXP)
    {
        _level = level;
        if (resetXP)
            _experience = 0;
    }

    private LevelUpReward GetLevelUpReward()
    {
        return rewardsIndex.rewards[_level - 1];
    }

    // gets the amount of xp required for the next level
    public uint GetCurrentXPRequirement()
    {
        uint xpReq = (uint)((_level + 1) * XpRequirementIncrement);
        //Debug.Log(_level + " + 1 * " + XpRequirementIncrement + " = " + xpReq);
        // ex level 3 = (3 + 1) * 1000 = 4000. requirement for level 4 is 4000 xp
        return xpReq;
    }
}
