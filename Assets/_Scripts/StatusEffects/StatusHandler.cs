using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StatusHandler : MonoBehaviour, IUpdateCustom {
    public event Action<StatusModifiersList> OnConstModifierChanged;
    public event Action<StatusModifiersList> OnDynamicModifierApplied;

    // --- EFFECTS ---
    private readonly List<StatusEffect> _temporaryEffects = new List<StatusEffect>();
    private readonly List<StatusEffect> _permanentEffects = new List<StatusEffect>();
    private List<StatusEffect> ChooseList(bool temp) => temp ? _temporaryEffects : _permanentEffects;
    private StatusModifiersList _modifiers;

    private void Awake() {
        _modifiers = new StatusModifiersList();
    }

    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) { }

    public void FixedUpdateCustom(float deltaTime, int tick = 0) {
        ProcessEffects(deltaTime, tick, false);
        ProcessEffects(deltaTime, tick);
    }

    // ------ EFFECT METHODS ------
    
    private void ProcessEffects(float deltaTime, int tick, bool isTemp = true) {
        List<StatusEffect> effects = ChooseList(isTemp);

        for (int i = 0; i < effects.Count; i++) {
            StatusEffect effect = effects[i];
            StatusModifiersList modifiers = effect.Modifiers;

            effect.FixedUpdateCustom(deltaTime, tick);

            if (modifiers.HasDynamicModifiers && effect.TickedThisFrame) OnDynamicModifierApplied?.Invoke(modifiers);

            if (effect.Expired) {
                if (isTemp) RemoveEffect(i);
                else effect.Refresh();
            }
        }
    }

    public void TryAddEffect(DataStatusEffect effect, bool isTemp = true) {
        if (!effect.stackable) {
            StatusEffect existing = GetFirstEffect(effect.name, isTemp);
            if (existing.IsEmpty) AddEffect(effect, isTemp);
            else if (effect.duration > existing.TimeLeft) existing.Refresh(effect.duration);
            return;
        }
        AddEffect(effect, isTemp);
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
        _modifiers.ResetList();
        
        if(_permanentEffects.Count > 0)
            _modifiers.UpdateList(_permanentEffects.ToArray(), false, false);
        if(_temporaryEffects.Count > 0)
            _modifiers.UpdateList(_temporaryEffects.ToArray(), false);

        OnConstModifierChanged?.Invoke(_modifiers);
    }


    // ------ HELPER METHODS ------

    public float ModConstant(AffectorConstType type, float baseConstant) => _modifiers.ModifyGivenConstant(type, baseConstant);
    public float GetDynamicModifier(AffectorDynamicType type) => _modifiers.GetDynamicModifier(type);


    public bool HasEffect(string effectName, bool isTemp = true) => ChooseList(isTemp).Any(t => t.CompareName(effectName));
    public StatusEffect GetFirstEffect(string effectName, bool isTemp = true) => ChooseList(isTemp).First(t => t.CompareName(effectName));
    public StatusEffect[] GetEffects(string effectName, bool isTemp = true) => ChooseList(isTemp).Where(t => t.CompareName(effectName)).ToArray();
}