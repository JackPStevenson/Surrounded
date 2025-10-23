using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject loadoutScreen;
    [SerializeField] private GameObject choosingItemScreen;
    [SerializeField] private GameObject mainMenuScreen;
    [SerializeField] private GameObject howToPlayScreen;
    [SerializeField] private GameObject settingsScreen;
    [SerializeField] private GameObject playerLevelScreen;
    [SerializeField] private GameObject shopScreen;
    [SerializeField] private GameObject lootBoxScreen;

    public void SetLoadoutScreen(bool active)
    {
        loadoutScreen.SetActive(active);
    }

    public void SetMainMenuScreen(bool active)
    {
        mainMenuScreen.SetActive(active);
    }

    public void SetHowToPlayScreen(bool active)
    {
        howToPlayScreen.SetActive(active);
    }

    public void SetSettingsScreen(bool active)
    {
        settingsScreen.SetActive(active);
    }

    public void SetPlayerLevelScreen(bool active)
    {
        playerLevelScreen.SetActive(active);
        //playerLevelScreen.GetComponent<LevelingScreenManager>().
    }

    public void SetLootBoxScreen(bool active)
    {
        lootBoxScreen.SetActive(active);
    }

    public void SetShopScreen(bool active)
    {
        shopScreen.SetActive(active);
    }
}
