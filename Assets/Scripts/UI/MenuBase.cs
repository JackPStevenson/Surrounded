using System;
using UnityEngine;

public class MenuBase : MonoBehaviour {
    public RectTransform Rect { get; private set; }
    public Vector2 Position { get => Rect.anchoredPosition; set => Rect.anchoredPosition = value; }
    
    // ------ START METHODS ------
    
    void Awake() {
        Rect = GetComponent<RectTransform>();
    }
    
    void Start() {
        
    }
    
    // ------ UPDATE METHODS ------
    
    void Update() {
        
    }
    
    // ------ EVENT METHODS ------
    
    public void Enable() => SetActive(true);
    public void Disable() => SetActive(false);
    public void SetActive(bool isActive) => gameObject.SetActive(isActive);
}