using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItem : MonoBehaviour {
    private TMP_Text _nameText;
    protected TMP_Text Name => ChildAutoFetch(_nameText, "Name");
    
    private TMP_Text _descText;
    protected TMP_Text Desc => ChildAutoFetch(_descText, "Desc");
    
    private Button _button;
    public Button Button => AutoFetch(_button);
    
    private Image _iconDisplay;
    // Icon image should be parented to either this script's transform or a child of it with name IconBorder.
    public Image Icon => ChildAutoFetch(_iconDisplay, "Icon") ?? ChildAutoFetch(_iconDisplay, "Icon", transform.Find("IconBorder"));

    // ------ SETUP METHODS ------
    
    public virtual void SetInfo(DataDisplayable d, bool allowDescLock = false) {
        bool unlocked = ManagerSaveLoad.CheckLevel(d.levelToUnlock) || allowDescLock;
        SetInfo(d.displayName, unlocked ? d.description : "Locked (Lvl " + d.levelToUnlock  + ")", d.icon, unlocked);
    }
    public void SetInfo(string name, string desc, Sprite icon, bool buttonActive = false) { SetName(name); SetDesc(desc); SetIcon(icon); SetButton(buttonActive); }

    // ------ SINGLE SETUP METHODS ------

    public void SetName(string name) => Name?.SetText(name);
    public void SetDesc(string desc) => Desc?.SetText(desc);
    public void SetButton(bool active) { if(Button) Button.interactable = active; }
    public void SetIcon(Sprite icon) { if (Icon) { Icon.enabled = icon; Icon.sprite = icon; } }

    // ------ HELPER METHODS ------
    
    protected T AutoFetch<T>(T compVar) where T : Component => AutoFetch(compVar, transform);
    protected T AutoFetch<T>(T compVar, Transform holder) where T : Component { if(!compVar) holder?.TryGetComponent(out compVar); return compVar; }
    
    protected T ChildAutoFetch<T>(T compVar, string child) where T : Component => ChildAutoFetch(compVar, child, transform);
    protected T ChildAutoFetch<T>(T compVar, string child, Transform parent) where T : Component { if (!compVar) parent?.Find(child)?.TryGetComponent(out compVar); return compVar; }
}