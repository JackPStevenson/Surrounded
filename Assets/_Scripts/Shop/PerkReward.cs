using UnityEngine;

[CreateAssetMenu(fileName = "PerkReward", menuName = "Scriptable Objects/PerkReward")]
public class PerkReward : Reward
{
    public override string Name => perk.displayName;
    public Sprite pIcon => perk.icon;
    public string Desc => perk.description;

    [SerializeField] private DataPerkPlayer perk;
    public override void GiveToPlayer(/*PerkInventory Inventory*/)
    {

        /*if (pInventory.IsUnlocked(perk))
        {
            switch (lootTier)
            {
                case 1:
                    // give player 1 tier 1 perk shard
                    break;
                case 2:
                    // give player 1 tier 2 perk shard
                    break;
                case 3:
                    // give player 1 tier 3 perk shard
                    break;
                default:
                    Debug.Log(lootTier + " is an invalid tier");
                    break;
            }
        }*/
    }
}
