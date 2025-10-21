using UnityEngine;

public class UIManager : MonoBehaviour
{
    // ui screens
    [SerializeField] private GameObject loadoutScreen;
    [SerializeField] private GameObject choosingItemScreen;
    [SerializeField] private GameObject MainMenuScreen;
    [SerializeField] private GameObject HowToPlayScreen;
    [SerializeField] private GameObject SettingsScreen;

    public LoadoutDisplayer loadoutDisplayer;
    public AvailableItemsDisplayer availableItemsDisplayer;


    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    private void ChangeLoadoutNames()
    {

    }

    // fill 
    // move between scenes
    // move between screens
}
