using UnityEngine;

[CreateAssetMenu(fileName = "Reward", menuName = "Scriptable Objects/Reward")]
public class Reward : ScriptableObject
{

    public virtual string Name => name;

    [SerializeField] protected float chance = 0;

    public float Chance => chance;

    [SerializeField] protected int lootTier = 0;
    public int LootTier => lootTier;


    public virtual void GiveToPlayer(/*PerkInventory Inventory*/)
    {

    }
}
