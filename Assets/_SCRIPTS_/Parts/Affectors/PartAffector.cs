using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public abstract class PartAffector : Part
{
    // --- TARGETING ---
    private Health[] _targetComps = Array.Empty<Health>();
    public void SetTargetComps(Health[] comps) => _targetComps = comps;

    public UnityEvent<Health> EventAppliedEffect;

    [Header("Randomness")]
    [Range(0, 100)]
    public float baseApplicationChance = 100;
    private float _bonusApplicationChance;

    // ------ PART FUNCTIONS ------

    public void SetBonusChance(float bonus) => _bonusApplicationChance = bonus;

    protected abstract void OnCompAffect(Health comp);
    protected override void InvokeLogic()
    {
        foreach (Health comp in _targetComps)
        {
            if (comp && (Random.value < (baseApplicationChance + _bonusApplicationChance) / 100))
            {
                OnCompAffect(comp);
                EventAppliedEffect?.Invoke(comp);
            }
        }
    }

    public override void Reset()
    {
        _targetComps = Array.Empty<Health>();
        _bonusApplicationChance = 0;
    }
}