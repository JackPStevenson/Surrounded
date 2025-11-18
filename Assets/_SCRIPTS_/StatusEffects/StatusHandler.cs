using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusHandler : MonoBehaviour, IUpdateCustom {
    public event Action<StatusModifiersList> EventConstModifierChanged;
    public event Action<StatusModifiersList> EventDynamicModifierApplied;

    // --- EFFECTS ---
    private readonly List<StatusEffect> _temporaryEffects = new List<StatusEffect>();
    private readonly List<StatusEffect> _permanentEffects = new List<StatusEffect>();
    private List<StatusEffect> GetList(bool tempList) => tempList ? _temporaryEffects : _permanentEffects;
    private int GetEffectCount(bool tempList) => GetList(tempList).Count;
    private StatusModifiersList _combinedModifiers;

    private void Awake() {
        _combinedModifiers = new StatusModifiersList();
    }

    // ------ UPDATE METHODS ------
    
    public void UpdateCustom(float deltaTime) { }
    public void FixedUpdateCustom(float deltaTime, int tick = 0) {
        ProcessEffects(deltaTime, tick, false);
        ProcessEffects(deltaTime, tick);
    }
    
    private void ProcessEffects(float deltaTime, int tick, bool isTemp = true) {
        List<StatusEffect> effects = GetList(isTemp);

        for (int i = 0; i < effects.Count; i++) {
            StatusEffect effect = effects[i];
            StatusModifiersList modifiers = effect.Modifiers;

            effect.FixedUpdateCustom(deltaTime, tick);

            if (modifiers.HasDynamicModifiers && effect.TickedThisFrame) EventDynamicModifierApplied?.Invoke(modifiers);

            if (!effect.Expired) continue;
            if (isTemp) {
                RemoveEffect(i);
                i--;
            }
            else effect.Refresh();
        }
        
        //_modifiers.Printt();
    }

    // ------ EFFECT METHODS ------

    public void TryAddEffect(DataStatusEffect newEffect, bool isTemp = true, float potency = 1) {
        if (newEffect.stackable) AddEffect(newEffect, isTemp, potency);
        else if (!HasEffect(newEffect.name, out StatusEffect found, isTemp)) AddEffect(newEffect, isTemp, potency);
        else if (newEffect.duration > found.TimeLeft) found.Refresh(newEffect.duration);
    }

    private void AddEffect(DataStatusEffect newEffect, bool isTemp = true, float potency = 1) {
        GetList(isTemp).Add(new StatusEffect(newEffect, potency));
        UpdateModifiers();
    }

    private void RemoveEffect(int index, bool isTemp = true) {
        GetList(isTemp).RemoveAt(index);
        UpdateModifiers();
    }

    public void Reset() {
        _permanentEffects.Clear();
        _temporaryEffects.Clear();
        UpdateModifiers();
    }
    
    // ------ MODIFIER METHODS ------

    private void UpdateModifiers() {
        _combinedModifiers.UpdateListFromEffects(GetList(false), GetList(true));

        EventConstModifierChanged?.Invoke(_combinedModifiers);
    }

    // ------ HELPER METHODS ------

    public float ModConst(AffectorConstType type, float val, bool subtract = false, bool clampMin = false) => _combinedModifiers.ApplyConstModifier(type, val, subtract, clampMin);
    public float GetDynamic(AffectorDynamicType type) => _combinedModifiers.GetDynamicModifier(type);

    public bool HasEffect(string effect, bool isTemp = true) => GetList(isTemp).Any(t => t.CompareName(effect));
    public bool HasEffect(string effect, out StatusEffect found, bool isTemp = true) => (found = GetList(isTemp).FirstOrDefault(t => t.CompareName(effect))) != null;
    public StatusEffect[] GetEffectsByName(string effectName, bool isTemp = true) => GetList(isTemp).Where(t => t.CompareName(effectName)).ToArray();
}