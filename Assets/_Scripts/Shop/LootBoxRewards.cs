using UnityEngine;

[CreateAssetMenu(fileName = "LootBoxRewards", menuName = "Scriptable Objects/LootBoxRewards")]
public class LootBoxRewards : ScriptableObject
{
    public PerkReward[] rewards;
}
