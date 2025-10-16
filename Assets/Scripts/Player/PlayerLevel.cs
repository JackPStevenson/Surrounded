using UnityEngine;

public class PlayerLevel : MonoBehaviour
{

    private uint _level = 0;
    private uint _experience = 0;

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
        uint xpReq = GetCurrentXPRequirement();
        if (_experience >= xpReq)
        {
            // level up
            LevelUp();
        }
    }

    // increases the player level and removes the required amount of experience
    private void LevelUp()
    {
        // remove xp from the player
        _experience -= GetCurrentXPRequirement();
        // increment level
        SetLevel(_level + 1, false);
    }

    // Sets the player level and if resetXP is true, resets the player's xp to zero
    public void SetLevel(uint level, bool resetXP)
    {
        _level = level;
        if (resetXP)
            _experience = 0;
    }

    // gets the amount of xp required for the next level
    private uint GetCurrentXPRequirement()
    {
        // ex level 3 = (3 + 1) * 1000 = 4000. requirement for level 4 is 4000 xp
        return (uint)((_level + 1) * XpRequirementIncrement);
    }
}
