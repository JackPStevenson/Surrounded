using System;
using UnityEngine;
using UnityEngine.UI;

public class FillableGraphic : MonoBehaviour {
    private readonly static int Fill = Shader.PropertyToID("_Fill");
    private const float ScaleTime = 0.35f;
    
    private RectTransform _rect;
    
    [Header("References")]
    public Image[] fillImages;

    [Header("Scaling")]
    public float selectedSizeMultiplier = 1.2f;
    private Vector2 _startSize;
    private float _sizeInterp = 0;
    
    bool _isEnlarged = false;

    private void Start() {
        _rect = GetComponent<RectTransform>();
        _startSize = _rect.sizeDelta;
    }

    // ----- UPDATE METHODS ------

    void Update() {
        // Only update size if neccesary.
        if (Mathf.Approximately(_sizeInterp, 1) && _isEnlarged) return;
        if (Mathf.Approximately(_sizeInterp, 0) && !_isEnlarged) return;
        
        float delta = Time.deltaTime * ScaleTime * (_isEnlarged ? 1 : -1);
        _sizeInterp = Mathf.Clamp01(_sizeInterp + delta);
        
        _rect.sizeDelta = Vector2.Lerp(_rect.sizeDelta, _startSize, Mathf.SmoothStep(0, 1, _sizeInterp));
    }

    // ----- HELPER METHODS ------

    /// Sets fill value for graphic.
    public void SetFill(float value) {
        foreach (Image t in fillImages) t.material.SetFloat(Fill, value);
    }

    /// Toggles whether graphic should be enlarged.
    public void ToggleEnlarged(bool enlarged) => _isEnlarged = enlarged;
}