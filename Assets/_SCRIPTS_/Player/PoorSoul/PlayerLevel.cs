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
        (_level, _experience) = PlayerData.LoadOrCreate();
    }

    // amt of xp that each level will require
    [SerializeField] private int XpRequirementIncrement = 1000;

    private void OnApplicationQuit()
    {
        // Save data when the app closes
        PlayerData.Save(_level, _experience);
    }

    private void OnApplicationPause(bool paused)
    {
        if (paused) PlayerData.Save(_level, _experience);
    }

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
        PlayerData.Save(_level, _experience); // save whenever XP changes
    }

    public void SetExperience(uint amt, bool attemptLevelUp)
    {
        _experience = amt;
        if (attemptLevelUp)
            AttemptLevelUp();
        PlayerData.Save(_level, _experience);
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
        PlayerData.Save(_level, _experience);
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

public static class PlayerData
{
    private const string K_LEVEL = "player_level";
    private const string K_EXPERIENCE = "player_experience";

    public static void Save(uint _level, uint _experience)
    {
        Debug.Log("Saved level as: " + _level + " and experience as " + _experience);
        PlayerPrefs.SetInt(K_LEVEL, unchecked((int)_level));
        PlayerPrefs.SetInt(K_EXPERIENCE, unchecked((int)_experience));
        PlayerPrefs.Save();
    }

    public static (uint _level, uint _experience) Load()
    {
        uint _level = (uint)PlayerPrefs.GetInt(K_LEVEL, 0);
        uint _experience = (uint)PlayerPrefs.GetInt(K_EXPERIENCE, 0);
        return (_level, _experience);
    }

    public static (uint _level, uint _experience) LoadOrCreate()
    {
        if (!PlayerPrefs.HasKey(K_LEVEL) || !PlayerPrefs.HasKey(K_EXPERIENCE))
        {
            uint defaultLevel = 0;
            uint defaultExperience = 0;
            Save(defaultLevel, defaultExperience);
            Debug.Log("Created new player data");
            return (defaultLevel, defaultExperience);
        }

        return Load();
    }
}