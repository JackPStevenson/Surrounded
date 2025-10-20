using System;
using System.Collections.Generic;
using UnityEngine;

public class HealthFlash : MonoBehaviour {
    // --- MATERIAL VARIABLES ---
    private readonly static int LastFlash = Shader.PropertyToID("_LastFlash");
    private readonly static int FlashColor = Shader.PropertyToID("_FlashColor");
    
    protected Health Health;
    
    // --- RENDERERS ---
    [Header("References")]
    public List<MeshRenderer> renderers;
    public List<SkinnedMeshRenderer> skinnedRenderers;

    // ------ START METHODS ------
    
    private void Start() {
        if (!Health && TryGetComponent(out Health h)) Initialize(h);
    }

    public void Initialize(Health h) {
        if (Health) Push();
        Health = h;
        Health.OnModHealth += FlashVisuals;
    }

    private void Push() {
        Health.OnModHealth -= FlashVisuals;
    }

    // ------ EVENTS ------
    
    /// Flashes linked mesh renderer materials. Sets color to either red or green based on whether health change is negative.
    private void FlashVisuals(float change) {
        Color c = change < 0 ? Color.red : Color.green;
        foreach (MeshRenderer r in renderers) if(r) Flash(r.material, c);
        foreach (SkinnedMeshRenderer r in skinnedRenderers) if(r) Flash(r.material, c);
    }

    private static void Flash(Material m, Color c) {
        m.SetColor(FlashColor, c);
        m.SetFloat(LastFlash, Time.time);
    }
}