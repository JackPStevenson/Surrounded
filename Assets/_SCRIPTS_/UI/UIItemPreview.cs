using TMPro;
using UnityEngine;

public class UIItemPreview : MonoBehaviour {

    [Header("References")]
    public UIDisplayItem titleDisplay;
    public Transform statsHolder;
    public TMP_Text elaborateDesc;

    private UIDisplayItem _levelDisplay;
    private UIDisplayItem _damageDisplay;
    private UIDisplayItem _radiusDisplay;
    private UIDisplayItem _swipeLengthDisplay;
    private UIDisplayItem _penetrationDisplay;
    private UIDisplayItem _cooldownDisplay;
    
    private bool _awoken = false;

    // ------ START METHODS ------

    void Awake() { if(!_awoken) OnAwake(); }
    private void OnAwake() {
        statsHolder.Find("Level").TryGetComponent(out _levelDisplay);
        statsHolder.Find("Damage").TryGetComponent(out _damageDisplay);
        statsHolder.Find("Radius").TryGetComponent(out _radiusDisplay);
        statsHolder.Find("SwipeLength").TryGetComponent(out _swipeLengthDisplay);
        statsHolder.Find("Penetration").TryGetComponent(out _penetrationDisplay);
        statsHolder.Find("Cooldown").TryGetComponent(out _cooldownDisplay);
    }

    // ------ EVENT METHODS ------

    public void PreviewItem(IDisplayable item, bool displayLevel = false) {
        gameObject.SetActive(true);
        if(!_awoken) OnAwake();
        
        // Initialize title and description with generic info from item.
        titleDisplay.SetInfo(item.GetData());
        
        if (item is SaveLoadPerk perk) titleDisplay.SetInfoFromPerk(perk, false);
        else titleDisplay.SetInfo(item.GetData(), true);
        
        elaborateDesc.SetText(item.GetElaborateDescription());
        
        // Toggle stats panel based on whether item is a weapon.
        bool isWeapon = item.GetData() is DataWeapon;
        statsHolder.gameObject.SetActive(isWeapon);
        if (!isWeapon) return;
        DataWeapon weapon = (DataWeapon) item.GetData();
        
        // If desired, display weapon's unlock level.
        if(displayLevel) _levelDisplay.SetDesc(weapon.levelToUnlock.ToString());
        _levelDisplay.gameObject.SetActive(displayLevel);
        
        // Fill out generic weapon data.
        _damageDisplay.SetDesc(weapon.damage.ToString("N0"));
        
        // If weapon has a cooldown, display it.
        float cooldown = Mathf.Ceil(weapon.energyCost / Mathf.Max(weapon.energyRegenRate, 0.001f) * 10) / 10;
        _cooldownDisplay.SetDesc(cooldown > 0 ? cooldown.ToString("N1") : "None");
        
        // If weapon is a tap weapon, display its hit type.
        if(weapon is DataWeaponTap tap)_penetrationDisplay.SetDesc(tap.penetration == 1 ? "Single" : "Multiple");
        _penetrationDisplay.gameObject.SetActive(weapon.CheckTypeId(0));

        // If weapon is a swipe weapon, display its swipe length.
        if(weapon is DataWeaponSwipe swipe) _swipeLengthDisplay.SetDesc(swipe.maxPathDistance.ToString("N1"));
        _swipeLengthDisplay.gameObject.SetActive(weapon.CheckTypeId(1));

        // If weapon is not a shake weapon, display its radius and penetration.
        if (!weapon.CheckTypeId(2)) _radiusDisplay.SetDesc(weapon.range.ToString("N1"));
        _radiusDisplay.gameObject.SetActive(!weapon.CheckTypeId(2));
    }

    public void Close() => gameObject.SetActive(false);
}
