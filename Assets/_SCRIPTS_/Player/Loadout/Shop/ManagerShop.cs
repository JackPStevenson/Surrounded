using System.Collections;
using TMPro;
using UnityEngine;

public class ManagerShop : MonoSingleton<ManagerShop> {
    private delegate bool Buy();
    private Buy buy;

    [SerializeField] private GameObject lowFundsMsg;

    private Coroutine lowFundsEnumerator;

    [SerializeField] private int LootBoxCost = 1000;

    [SerializeField] private int[] TrapCosts = {100, 250, 400, 600};

    [SerializeField] private TMP_Text PlayerZombieBlood;

    [SerializeField] private GameObject[] TrapButtons;
    [SerializeField] private GameObject LootboxButton;
    [SerializeField] private GameObject BuyScreen;

    // ------ START METHODS ------

    protected override void OnAwake() { }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    private void Start() => SetUpShop();

    public void SetUpShop() {
        // display Player zombie blood
        UpdateZombieBloodDisplay();
        // initialize prices for lootbox and traps
        LootboxButton.transform.GetChild(2).GetComponent<TMP_Text>().text = LootBoxCost + " ZB";
        for (int i = 0; i < TrapButtons.Length; i++) {
            TrapButtons[i].transform.GetChild(2).GetComponent<TMP_Text>().text = TrapCosts[i] + " ZB";
        }
    }
    
    // ------ EVENT METHODS ------

    public void SetBuyScreen(string name, /*Sprite sprite,*/ int cost) {
        BuyScreen.SetActive(true);
        BuyScreen.transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = "Buy " + name + "?";
        BuyScreen.transform.GetChild(0).GetChild(2).GetChild(0).GetComponent<TMP_Text>().text = cost + " ZB";

    }

    public void SetBuyAction(string buyAction) {
        switch (buyAction) {
            case "lootbox":
                buy = PurchaseLootBox;
                SetBuyScreen("lootbox", (int) LootBoxCost);
                break;
            default:
                break;
        }
    }

    public void ActivateBuy() {
        if (buy == null) return;
        if (buy()) {
            BuyScreen.SetActive(false);
            UpdateZombieBloodDisplay();
        }
        else {
            // display insuffficent funds text
            if (lowFundsEnumerator != null)
                StopCoroutine(lowFundsEnumerator);
            lowFundsEnumerator = StartCoroutine(DisplayInsufficientFundsMessage());
        }
    }

    public void UpdateZombieBloodDisplay() {
        PlayerZombieBlood.text = "Zombie Blood: " + ManagerSaveLoad.GetZombieBlood();
    }

    public bool PurchaseLootBox() {
        if (ManagerSaveLoad.TrySpendZombieBlood(LootBoxCost)) {
            LootboxManager.instance.OpenLootBoxScreen();
            UpdateZombieBloodDisplay();
            return true;
        }
        return false;
    }

    private IEnumerator DisplayInsufficientFundsMessage() {
        lowFundsMsg.SetActive(true);
        yield return new WaitForSeconds(1);
        lowFundsMsg.SetActive(false);
    }
}