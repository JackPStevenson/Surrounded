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
        // loadoutScreen.GetComponent<UILoadout>().LoadLoadout();
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
        if (active)
            LevelingScreenManager.instance.UpdateLevelDisplay();
    }

    public void SetLootBoxScreen(bool active)
    {
        lootBoxScreen.SetActive(active);
    }

    public void SetShopScreen(bool active)
    {
        shopScreen.SetActive(active);
    }

    public void PlayClickSound()
    {
        AudioManager.PlaySoundByIndex(SoundType.UI, 0, 1f);
    }

    public void PlayLootBoxOpenedSound()
    {
        // AudioManager.PlaySoundByIndex(SoundType.UI, 1, 1f);
    }

    public void PlayItemClaimSound()
    {
        // AudioManager.PlaySoundByIndex(SoundType.UI, 2, 1f);
    }
}
