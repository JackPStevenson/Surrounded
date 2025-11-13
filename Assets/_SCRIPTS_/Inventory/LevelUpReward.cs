using UnityEngine;

public enum RewardType
{
    ZombieBlood,
    TapWeapon,
    SwipeWeapon,
    ShakeWeapon,
    Lootbox,
    Trap
}

[CreateAssetMenu(fileName = "LevelUpReward", menuName = "LevelUpReward")]
public class LevelUpReward : ScriptableObject
{
    public RewardType type;
    public int ZombieBloodReward = 0;
    public DataDisplayable itemToAdd = null;
    public bool claimed = false;
}

