using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItem : MonoBehaviour {
    private TMP_Text nameText;
    protected TMP_Text Name { get { if (!nameText) transform.Find("Name")?.TryGetComponent(out nameText); return nameText; } }
    
    private TMP_Text descText;
    protected TMP_Text Desc { get { if (!descText) transform.Find("Desc")?.TryGetComponent(out descText); return descText; } }
    
    private Button button;
    public Button Button { get { if (!button) TryGetComponent(out button); return button; } }
    
    private Image iconDisplay;
    public Image Icon { 
        get {
            if (!iconDisplay && (!transform.Find("Icon") || !transform.Find("Icon").TryGetComponent(out iconDisplay)))
                transform.Find("IconBorder")?.Find("Icon")?.TryGetComponent(out iconDisplay);
            return iconDisplay;
        }
    }

    public void SetInfo(DataDisplayable d, int maxWaveReached) {
        bool waveReached = maxWaveReached >= d.minWaveToUnlock;
        SetInfo(d.displayName, waveReached ? d.description : "Reach level " + d.minWaveToUnlock, d.icon);
        Button.interactable = waveReached;
    }


    public void SetInfo(DataDisplayable d) => SetInfo(d.displayName, d.description, d.icon);
    public void SetInfo(string name, string desc, Sprite icon) { SetName(name); SetDesc(desc); SetIcon(icon); }

    public void SetName(string name) => Name?.SetText(name);
    public void SetDesc(string desc) => Desc?.SetText(desc);
    public void SetIcon(Sprite icon) {
        if (!Icon) return;
        Icon.enabled = icon;
        Icon.sprite = icon;
    }
}