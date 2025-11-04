using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LootboxManager : MonoBehaviour
{
    public static LootboxManager instance;

    [SerializeField] private LootBoxRewards rewards;

    [SerializeField] private Button lootBoxButton;

    [SerializeField] private GameObject lootBoxCanvas;

    [SerializeField] private Transform PerkSummaryScreen;
    private TMP_Text perkNameText;
    private TMP_Text perkTierText;
    private TMP_Text perkDescriptionText;
    private Image perkIcon;


    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != null & instance != this)
            Destroy(this);

    }

    public void OpenLootBoxScreen()
    {
        lootBoxButton.interactable = true;
        lootBoxCanvas.SetActive(true);
    }

    private void Start()
    {
        perkNameText = PerkSummaryScreen.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        perkTierText = PerkSummaryScreen.GetChild(1).GetChild(1).GetComponent<TMP_Text>();
        perkIcon = PerkSummaryScreen.GetChild(1).GetChild(2).GetComponent<Image>();
        perkDescriptionText = PerkSummaryScreen.GetChild(1).GetChild(3).GetComponent<TMP_Text>();
    }

    private PerkReward PickItem()
    {
        float totalChance = 0;
        foreach (PerkReward loot in rewards.rewards)
        {
            totalChance += loot.Chance;
        }

        float rand = Random.Range(0, totalChance);

        float tempSum = 0;

        foreach (PerkReward loot in rewards.rewards)
        {
            tempSum += loot.Chance;
            Debug.Log("TempSum:" + tempSum + "\nrand: " + rand);
            if (tempSum > rand)
            {
                return loot;
            }
        }

        return rewards.rewards[0];
    }

    public void OpenPerkLootBox()
    {
        PerkReward reward = PickItem();
        SetPerkSummaryScreen(reward.Name, reward.LootTier, reward.pIcon, reward.Desc);
        lootBoxButton.interactable = false;
    }

    private void SetPerkSummaryScreen(string name, int tier, Sprite icon, string description)
    {
        Debug.Log("SetPerkSummaryScreen");
        PerkSummaryScreen.gameObject.SetActive(true);
        perkNameText.text = name;
        perkTierText.text = "Tier " + tier + " Perk";
        perkIcon.sprite = icon;
        perkDescriptionText.text = description;
    }


}
