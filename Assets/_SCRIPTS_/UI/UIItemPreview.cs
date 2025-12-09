using TMPro;
using UnityEngine;

public class UIItemPreview : MonoBehaviour {
    private UIDisplayItem _titleDisplay;

    private Transform _contextHolder;
    private Transform _stats;
    private TMP_Text _elaborateDesc;

    private UIDisplayItem _damageDisplay;
    private UIDisplayItem _radiusDisplay;
    private UIDisplayItem _swipeLengthDisplay;
    private UIDisplayItem _penetrationDisplay;
    private UIDisplayItem _cooldownDisplay;
    
    private bool _awoken = false;

    // ------ START METHODS ------

    void Awake() { if(!_awoken) OnAwake(); }

    private void OnAwake() {
        _titleDisplay = GetComponentInChildren<UIDisplayItem>();

        _contextHolder = transform.Find("Context");
        _stats = _contextHolder.Find("Stats");
        _contextHolder.Find("Elaborate").TryGetComponent(out _elaborateDesc);

        _stats.Find("Damage").TryGetComponent(out _damageDisplay);
        _stats.Find("Radius").TryGetComponent(out _radiusDisplay);
        _stats.Find("SwipeLength").TryGetComponent(out _swipeLengthDisplay);
        _stats.Find("Penetration").TryGetComponent(out _penetrationDisplay);
        _stats.Find("Cooldown").TryGetComponent(out _cooldownDisplay);
    }

    // ------ EVENT METHODS ------

    public void PreviewItem(DataDisplayable item) {
        
        gameObject.SetActive(true);
        if(!_awoken) OnAwake();
        
        // Initialize title and description with generic info from item.
        _titleDisplay.SetInfo(item);
        _elaborateDesc.SetText(item.elaborateDescription);
        
        // Toggle stats panel based on whether item is a weapon.
        DataWeapon weapon = (DataWeapon) item;
        _stats.gameObject.SetActive(weapon);
        if (!weapon) return;
        
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
