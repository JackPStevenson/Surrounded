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
    
    // ------ START METHODS ------
    
    void Awake() {
        _titleDisplay = GetComponentInChildren<UIDisplayItem>();

        _contextHolder = transform.Find("Context");
        _stats = _contextHolder.Find("Stats");
        transform.Find("Elaborate").TryGetComponent(out _elaborateDesc);
        
        _stats.Find("Damage").TryGetComponent(out _damageDisplay);
        _stats.Find("Radius").TryGetComponent(out _radiusDisplay);
        _stats.Find("SwipeLength").TryGetComponent(out _swipeLengthDisplay);
        _stats.Find("Penetration").TryGetComponent(out _penetrationDisplay);
        _stats.Find("Cooldown").TryGetComponent(out _cooldownDisplay);
    }
    
    // ------ EVENT METHODS ------

    public void PreviewItem(DataDisplayable item) {
        _titleDisplay.SetInfo(item);
        _elaborateDesc.SetText(item.elaborateDescription);
        
        
        if (item is DataWeapon weapon) {
            _damageDisplay.gameObject.SetActive(true);
            _cooldownDisplay.gameObject.SetActive(true);
            
            _swipeLengthDisplay.gameObject.SetActive(weapon.CheckTypeId(1));
            
            _radiusDisplay.gameObject.SetActive(!weapon.CheckTypeId(2));
            _penetrationDisplay.gameObject.SetActive(!weapon.CheckTypeId(2));
            
            _stats.gameObject.SetActive(true);
        }
        else {
            _stats.gameObject.SetActive(false);
            
        }
    }
}
