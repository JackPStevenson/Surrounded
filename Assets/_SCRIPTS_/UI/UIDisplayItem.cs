using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItem : MonoBehaviour {
    private TMP_Text _nameText;
    protected TMP_Text Name => ChildAutoFetch(_nameText, "Name");
    
    private TMP_Text _descText;
    protected TMP_Text Desc => ChildAutoFetch(_descText, "Desc");
    
    private TMP_Text _extraText;
    protected TMP_Text ExtraText => ChildAutoFetch(_descText, "ExtraText");
    
    private Button _button;
    public Button Button => AutoFetch(_button);
    
    private Image _iconDisplay;
    // Icon image should be parented to either this script's transform or a child of it with name IconBorder.
    public Image Icon => ChildAutoFetch(_iconDisplay, "Icon") ?? ChildAutoFetch(_iconDisplay, "Icon", transform.Find("IconBorder"));

    // ------ SETUP METHODS ------
    
    public virtual void SetInfo(DataDisplayable d, bool allowLock = false) => SetInfo(d.displayName, d.description, d.icon, d.levelToUnlock, allowLock);
    public void SetInfo(IDisplayable d, bool allowLock = false) => SetInfo(d.GetDisplayName(), d.GetDescription(), d.GetIcon(), d.GetLevelToUnlock(), allowLock);
    public void SetInfo(string name, string desc, Sprite icon = null, int level = 0, bool allowLock = false) { gameObject.SetActive(true); SetName(name); SetDesc(desc, level, allowLock); SetIcon(icon); SetButton(level, allowLock); SetExtraText(""); }
    public void SetInfoFromPerk(SaveLoadPerk perk, bool usedInOtherSlot) { SetInfo(perk); SetButton(!usedInOtherSlot); if(usedInOtherSlot) SetDesc("(In Use)"); SetExtraText(perk.perkPower * 2 + ""); }

    // ------ SINGLE SETUP METHODS ------

    public void SetName(string name) => Name?.SetText(name);
    public string GetName() => Name?.text;
    public void SetDesc(string desc, int level = 0, bool allowLock = false) => Desc?.SetText(!allowLock || ManagerSaveLoad.CheckLevel(level) ? desc : "Locked (Level " + level  + ")");
    public void SetButton(int level, bool allowLock) => SetButton(!allowLock || ManagerSaveLoad.CheckLevel(level));
    public void SetButton(bool active) { if(Button) Button.interactable = active; }
    public void SetIcon(Sprite icon) { if (Icon) { Icon.enabled = icon; Icon.sprite = icon; } }
    public void SetDescColor(Color color) { if (Desc) Desc.color = color; }
    public void SetExtraText(string text) => ExtraText?.SetText(text);

    // ------ HELPER METHODS ------
    
    protected T AutoFetch<T>(T compVar) where T : Component => AutoFetch(compVar, transform);
    protected T AutoFetch<T>(T compVar, Transform holder) where T : Component { if(!compVar) holder?.TryGetComponent(out compVar); return compVar; }
    
    protected T ChildAutoFetch<T>(T compVar, string child) where T : Component => ChildAutoFetch(compVar, child, transform);
    protected T ChildAutoFetch<T>(T compVar, string child, Transform parent) where T : Component { if (!compVar) parent?.Find(child)?.TryGetComponent(out compVar); return compVar; }
}