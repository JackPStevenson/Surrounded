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
    private StatusModifiersList _modifiers;

    private void Awake() {
        _modifiers = new StatusModifiersList();
    }

    // ------ UPDATE METHODS ------

    public void FixedUpdateCustom(float deltaTime, int tick = 0) {
        ProcessEffects(deltaTime, tick, false);
        ProcessEffects(deltaTime, tick);
    }

    // ------ EFFECT METHODS ------
    
    private List<StatusEffect> ChooseList(bool temp) => temp ? _temporaryEffects : _permanentEffects;
    
    private void ProcessEffects(float deltaTime, int tick, bool isTemp = true) {
        List<StatusEffect> effects = ChooseList(isTemp);

        for (int i = 0; i < effects.Count; i++) {
            StatusEffect effect = effects[i];
            StatusModifiersList modifiers = effect.Modifiers;

            effect.FixedUpdateCustom(deltaTime, tick);

            if (modifiers.HasDynamicModifiers && effect.TickedThisFrame) EventDynamicModifierApplied?.Invoke(modifiers);

            if (effect.Expired) {
                if (isTemp) RemoveEffect(i);
                else effect.Refresh();
            }
        }
    }

    public void TryAddEffect(DataStatusEffect newEffect, bool isTemp = true) {
        if (newEffect.stackable) AddEffect(newEffect, isTemp);
        else if (!HasEffect(newEffect.name, out StatusEffect found, isTemp)) AddEffect(newEffect, isTemp);
        else if (newEffect.duration > found.TimeLeft) found.Refresh(newEffect.duration);
    }

    private void AddEffect(DataStatusEffect newEffect, bool isTemp = true) {
        ChooseList(isTemp).Add(new StatusEffect(newEffect));
        UpdateModifiers();
    }

    private void RemoveEffect(int index, bool isTemp = true) {
        ChooseList(isTemp).RemoveAt(index);
        UpdateModifiers();
    }

    public void Reset() {
        _permanentEffects.Clear();
        _temporaryEffects.Clear();
        UpdateModifiers();
    }

    private void UpdateModifiers() {
        _modifiers.Reset();
        
        if(_permanentEffects.Count > 0) _modifiers.UpdateList(_permanentEffects.ToArray(), false, false);
        if(_temporaryEffects.Count > 0) _modifiers.UpdateList(_temporaryEffects.ToArray(), false);

        EventConstModifierChanged?.Invoke(_modifiers);
    }
    
    // ------ HELPER METHODS ------

    public float ModConst(AffectorConstType type, float baseConstant) => _modifiers.ModifyConstant(type, baseConstant);
    public float GetDynamic(AffectorDynamicType type) => _modifiers.GetDynamicModifier(type);

    public bool HasEffect(string effectName, bool isTemp = true) => ChooseList(isTemp).Any(t => t.CompareName(effectName));
    public bool HasEffect(string effectName, out StatusEffect found, bool isTemp = true) {
        found = ChooseList(isTemp).FirstOrDefault(t => t.CompareName(effectName));
        return found != null;
    }
    public StatusEffect[] GetEffectsByName(string effectName, bool isTemp = true) => ChooseList(isTemp).Where(t => t.CompareName(effectName)).ToArray();
}