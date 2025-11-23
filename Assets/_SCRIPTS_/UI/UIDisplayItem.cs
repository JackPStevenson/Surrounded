using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItem : MonoBehaviour {
    private TMP_Text _nameText;
    protected TMP_Text Name { get { if (!_nameText) transform.Find("Name")?.TryGetComponent(out _nameText); return _nameText; } }
    
    private TMP_Text _descText;
    protected TMP_Text Desc { get { if (!_descText) transform.Find("Desc")?.TryGetComponent(out _descText); return _descText; } }
    
    private Button _button;
    public Button Button { get { if (!_button) TryGetComponent(out _button); return _button; } }
    
    private Image _iconDisplay;
    public Image Icon { 
        get {
            if (!_iconDisplay && (!transform.Find("Icon") || !transform.Find("Icon").TryGetComponent(out _iconDisplay)))
                transform.Find("IconBorder")?.Find("Icon")?.TryGetComponent(out _iconDisplay);
            return _iconDisplay;
        }
    }

    public virtual void SetInfo(DataDisplayable d, bool allowDescLock = false) {
        bool unlocked = ManagerSaveLoad.CheckLevel(d.levelToUnlock) || allowDescLock;
        SetInfo(d.displayName, unlocked ? d.description : "Locked (Lvl " + d.levelToUnlock  + ")", d.icon, unlocked);
    }
    public void SetInfo(string name, string desc, Sprite icon, bool buttonActive = false) { SetName(name); SetDesc(desc); SetIcon(icon); SetButton(buttonActive); }

    public void SetName(string name) => Name?.SetText(name);
    public void SetDesc(string desc) => Desc?.SetText(desc);
    public void SetButton(bool active) { if(Button) Button.interactable = active; }
    public void SetIcon(Sprite icon) { if (Icon) { Icon.enabled = icon; Icon.sprite = icon; } }
}
