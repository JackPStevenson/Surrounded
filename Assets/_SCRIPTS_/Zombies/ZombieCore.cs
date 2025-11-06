using System;
using UnityEngine;

[RequireComponent(typeof(Health))] [RequireComponent(typeof(StatusHandler))] [RequireComponent(typeof(ZombieNav))]
public class ZombieCore : MonoBehaviour, IUpdateCustom {
    public event Action<ZombieCore> EventDeath;
    
    public int Id { get; private set; } = -1;
    
    // --- PERMANENT REFERENCES ---
    public Health Health => _health;
    private Health _health;
    public StatusHandler Status => _status;
    private StatusHandler _status;
    public ZombieNav Nav => _nav;
    private ZombieNav _nav;
    
    // --- TEMPORARY REFERENCES ---
    public DataZombie Data => _data;
    private DataZombie _data;
    public ZombieAnimator Anim => _anim;
    private ZombieAnimator _anim;
    private HealthFlash _flash;

    // --- BASE PARAMETERS ---
    public float Speed => Status.ModConst(AffectorConstType.Speed, _data.speed);
    public float Damage => Status.ModConst(AffectorConstType.Damage, _data.attackDamage);
    public float AttackRate => Status.ModConst(AffectorConstType.Speed, _data.attackRate);
    public float BaseHealth => Status.ModConst(AffectorConstType.Speed, _data.health);
    public float BaseRange => Status.ModConst(AffectorConstType.Speed, _data.attackRange);
    
    // --- STATE PARAMETERS ---
    public Vector3 Position => Health.Position;
    public Vector3 Velocity => Nav.Velocity;
    public Vector3 MoveDirection => Vector3.Scale(Velocity, new Vector3(1, 0, 1)).normalized;
    public Vector3 TargetDirection => Nav.TargetDirection;

    public bool IsAlive => _isAlive;
    private bool _isAlive = false;
    
    //public float CurrentResist => Status.ModConstant(AffectorConstType.BaseAttackDamage, Data.resistance);
    
    // ------ START METHODS ------
    
    public void Initialize(int id, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        TryGetComponent(out _health);
        TryGetComponent(out _status);
        TryGetComponent(out _nav);
        
        Id = id;
        gameObject.name = "Zombie " + id;
        Nav.Initialize(this, mainTarget, approachDist, sideTargetMask);
        
        Health.EventDeath += Despawn;
        Health.EventDeath += OnDeath;
        Status.EventDynamicModifierApplied += EventDynamicModifierApplied;
    }
    
    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) { }
    public void FixedUpdateCustom(float deltaTime, int tick) {
        Nav.FixedUpdateCustom(deltaTime, tick);
        Anim.FixedUpdateCustom(deltaTime, tick);
        Status.FixedUpdateCustom(deltaTime, tick);
    }
    
    // ------ POOLING ------
    
    public void Spawn(DataZombie dataZombie, Vector3 spawnPos) {
        if (Id < 0) {
            enabled = false;
            return;
        }
        
        _data = dataZombie;

        if (!_anim) {
            GameObject go = Instantiate(dataZombie.visualPrefab, transform);
            go.TryGetComponent(out _anim);
            go.TryGetComponent(out _flash);
        }
        else {
            Debug.Log("What are ya doin.");
        }

        Health.SetMaxHealth(_data.health);
        Health.Reset();
        
        Anim.Initialize(this);
        _flash.Initialize(Health);
        
        Nav.Spawn(spawnPos);
        _isAlive = true;
        SetActive(true);
    }

    void OnDeath() => EventDeath?.Invoke(this);
    
    /// Returns zombie back to pool with its data erased.
    public void Despawn() {
        _isAlive = false;
        
        Status.Reset();
        Nav.Reset();
        _data = null;

        Destroy(_anim.gameObject);
        _anim = null;
        _flash = null;
        
        SetActive(false);
    }
    
    public void SetActive(bool active) => gameObject.SetActive(active);

    private void EventDynamicModifierApplied(StatusModifiersList modifiers) {
        float healthModifier = modifiers.GetDynamicModifier(AffectorDynamicType.CurrentHealth);
        
        if (!Mathf.Approximately(healthModifier, 0))
            Health.DealDamage(healthModifier);
    }
}