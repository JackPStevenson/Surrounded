using UnityEngine;

public enum RewardType
{
    Blood,
    Lootbox,
    Perk
}

[CreateAssetMenu(fileName = "GenericReward", menuName = "Data/GenericReward")]
public class DataLevelGenericReward : DataDisplayable {
    [Header("Reward Type")]
    public RewardType type;
    public int amount = 0;
}
