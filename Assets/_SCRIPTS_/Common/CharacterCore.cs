using System;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(StatusHandler))]
public abstract class CharacterCore : MonoBehaviour, IUpdateCustom {
    public const string PerkScalarTag = "PerkScalar";
    
    public event Action<CharacterCore, string> EventDeath;

    // --- CORE REFERENCES ---
    private Health _health;
    private StatusHandler _status;
    public CharacterAudioPlayer _audioPlayer;

    public Health Health => _health;
    public StatusHandler Status => _status;
    public Vector3 Position => Health.Position;
    public bool IsAlive => _health.HealthRatio > 0;

    // ------ START METHODS ------

    protected void Initialize()
    {
        TryGetComponent(out _health);
        TryGetComponent(out _status);
        TryGetComponent(out _audioPlayer);

        _health.EventDeath += Deactivate;
        _status.EventDynamicModifierApplied += OnDynamicModifier;
    }

    // ------ UPDATE METHODS ------

    protected virtual void OnUpdateCustom(float deltaTime) { }
    public void UpdateCustom(float deltaTime)
    {
        OnUpdateCustom(deltaTime);
        _status.UpdateCustom(deltaTime);
    }

    protected virtual void OnFixedUpdateCustom(float deltaTime, int tick) { }
    public void FixedUpdateCustom(float deltaTime, int tick)
    {
        OnFixedUpdateCustom(deltaTime, tick);
        _status.FixedUpdateCustom(deltaTime, tick);
    }

    // ------ EVENT METHODS ------

    protected virtual void OnDeactivate() { }
    public void Deactivate(string deathSource = "")
    {
        _status.Reset();
        gameObject.SetActive(false);
        _audioPlayer?.PlayDeathSound();
        EventDeath?.Invoke(this, deathSource);
        OnDeactivate();
    }

    protected virtual void OnActivate() { }
    public void Activate()
    {
        _health.Reset();
        gameObject.SetActive(true);
        OnActivate();
    }

    public void TryAddEffect(DataStatusEffect newEffect, bool isTemp = true, float potency = 1) => Status.TryAddEffect(newEffect, isTemp, potency);

    public void TryAddPerk(DataPerkPlayer dataPerkPlayer, float scalar) {
        PartLogicValue part = Instantiate(dataPerkPlayer.perkPrefab, transform).GetComponents<PartLogicValue>().FirstOrDefault(p => p.Compare(PerkScalarTag));
        if (part) part.SetValue(scalar);
    }

    private void OnDynamicModifier(StatusModifiersList modifiers)
    {
        float healthModifier = modifiers.GetDynamicModifier(AffectorDynamicType.CurrentHealth);
        if (!Mathf.Approximately(healthModifier, 0)) _health.DealDamage(healthModifier);
    }
}