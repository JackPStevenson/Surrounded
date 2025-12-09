using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public enum SelectType { None = 0, WeaponTap = 1, WeaponSwipe = 2, WeaponShake = 3, Perk1 = 4, Perk2 = 5, Perk3 = 6 }

public class UIGridSelect : MonoBehaviour {
    public event Action<SelectType, int> OnSelected;

    [Header("References")]
    public GameObject displayItemPrefab;
    public ScrollRect scroller;
    public GameObject confirmButton;

    private UIItemPreview _itemPreview;
    private TMP_Text _chooseText;

    UIDisplayItem[] _items;
    
    public SelectType CurrentSelectType => _currentSelectType;
    public bool IsSelecting => _currentSelectType is not SelectType.None;
    private SelectType _currentSelectType = SelectType.None;
    private int _previewIndex = -1;
    
    public IDisplayable[] Selectables => (int) _currentSelectType switch { 1 => ManagerData.Taps, 2 => ManagerData.Swipes, 3 => ManagerData.Shakes, 4 or 5 or 6 => ManagerSaveLoad.GetPerkInstances(), _ => null };
    public string SelectableName => (int) _currentSelectType switch { 1 => "Tap Weapon", 2 => "Swipe Weapon", 3 => "Shake Weapon", 4 => "Perk 1", 5 => "Perk 2", 6 => "Perk 3", _ => "null" };

    // ------ START METHODS ------

    void Awake() {
        transform.Find("OptionsPanel")?.Find("ChooseText")?.TryGetComponent(out _chooseText);
        transform.Find("SelectPreview")?.TryGetComponent(out _itemPreview);
    }

    // ------ GRID MANAGEMENT METHODS ------

    public void Initialize(SelectType selectType) {
        if (!_itemPreview) Awake();
        _currentSelectType = selectType;
        
        // Get selectables and display them on grid.
        _items = new UIDisplayItem[Selectables.Length];
        for (int i = 0; i < Selectables.Length; i++) MakeDisplayItem(Selectables, i);

        // Enable display.
        if (_chooseText) _chooseText.text = "Choose a " + SelectableName;
        gameObject.SetActive(true);
        confirmButton.SetActive(false);
        
        Debug.Log(1);
        scroller.verticalScrollbar.value = 1;
    }

    private void PreviewItem(int index) {
        _previewIndex = index;
        _itemPreview.PreviewItem(Selectables[_previewIndex]);
        confirmButton.SetActive(true);
    }

    public void ConfirmSelect() {
        OnSelected?.Invoke(_currentSelectType, _previewIndex);
        Clear();
    }

    public void Clear() {
        for (int i = _items.Length - 1; i >= 0; i--) {
            UIDisplayItem item = _items[i];
            item.Button.onClick.RemoveAllListeners();
            Destroy(item.gameObject);
        }
        
        _previewIndex = -1;
        _currentSelectType = SelectType.None;
        _itemPreview.Close();
        
        _items = Array.Empty<UIDisplayItem>();
        gameObject.SetActive(false);
    }

    // ------ EVENT METHODS ------
    
    /// Makes display item from given displayable at given index in items array. Additionally, adds listener to display item's button.
    private void MakeDisplayItem<T>(T[] displayables, int index) where T : IDisplayable {
        GameObject e = Instantiate(displayItemPrefab, scroller.content);
        e.TryGetComponent(out UIDisplayItem i);
        i.SetInfo(displayables[index].GetData(), true);
        i.Button.onClick.AddListener(() => PreviewItem(index));
        _items[index] = i;
    }
}