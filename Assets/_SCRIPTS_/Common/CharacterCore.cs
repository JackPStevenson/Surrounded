using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(StatusHandler))]
public abstract class CharacterCore : MonoBehaviour, IUpdateCustom
{
    public event Action<CharacterCore> EventDeath;

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
        if (_audioPlayer != null)
            _health.EventDeath += _audioPlayer.PlayDeathSound;
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
    public void Deactivate()
    {
        OnDeactivate();

        _status.Reset();
        gameObject.SetActive(false);
        EventDeath?.Invoke(this);
    }

    protected virtual void OnActivate() { }
    public void Activate()
    {
        OnActivate();
        _health.Reset();
        gameObject.SetActive(true);
    }

    public void TryAddEffect(DataStatusEffect newEffect, bool isTemp = true, float potency = 1) => Status.TryAddEffect(newEffect, isTemp, potency);

    public void AddPerk(DataPerkPlayer dataPerkPlayer)
    {
        Instantiate(dataPerkPlayer.perkPrefab, transform);
    }

    private void OnDynamicModifier(StatusModifiersList modifiers)
    {
        float healthModifier = modifiers.GetDynamicModifier(AffectorDynamicType.CurrentHealth);
        if (!Mathf.Approximately(healthModifier, 0)) _health.DealDamage(healthModifier);
    }
}