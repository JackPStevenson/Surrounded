using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIGridSelect : MonoBehaviour {
    public event Action<int> OnSelected;

    [Header("References")]
    public GameObject displayItemPrefab;
    private Transform _grid;
    private TMP_Text _chooseText;

    UIDisplayItem[] _items;

    void Awake() {
        _grid = transform.Find("Grid");
        transform.Find("OptionsPanel")?.Find("ChooseText")?.TryGetComponent(out _chooseText);
    }

    // ------ GRID MANAGEMENT ------

    public void Initialize<T>(T[] displayables, string typeName) where T : DataDisplayable {
        if (!_grid) Awake();
        
        int maxWaveReached = PlayerPrefs.GetInt("MaxWaveReached", 1);
        _items = new UIDisplayItem[displayables.Length];
        for (int i = 0; i < displayables.Length; i++) {
            MakeDisplayItem(displayables, i, maxWaveReached);
        }

        if (_chooseText) _chooseText.text = "Choose a " + typeName;
        gameObject.SetActive(true);
    }

    public void ClearGrid() {
        for (int i = _items.Length - 1; i >= 0; i--) {
            UIDisplayItem item = _items[i];
            item.Button.onClick.RemoveAllListeners();
            Destroy(item.gameObject);
        }
        
        _items = Array.Empty<UIDisplayItem>();
        gameObject.SetActive(false);
    }

    // ------ EVENTS ------

    public List<GameObject> Items = new List<GameObject>();
    /// Makes display item from given displayable at given index in items array. Additionally adds listener to display item's button.
    private void MakeDisplayItem<T>(T[] displayables, int index, int maxWaveReached) where T : DataDisplayable {
        GameObject e = Instantiate(displayItemPrefab, _grid);
        e.TryGetComponent(out UIDisplayItem i);
        Items.Add(e);
        i.SetInfo(displayables[index], maxWaveReached);
        i.Button.onClick.AddListener(() => Select(index));
        _items[index] = i;
    }

    private void Select(int index) {
        OnSelected?.Invoke(index);
        ClearGrid();
    }
}