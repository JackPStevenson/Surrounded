using System;
using UnityEngine;

[RequireComponent(typeof(Health))] [RequireComponent(typeof(StatusHandler))] [RequireComponent(typeof(ZombieNav))]
public class ZombieCore : MonoBehaviour, IUpdateCustom {
    protected ManagerZombies Manager;
    
    public int Id { get; private set; } = -1;
    
    // --- PERMANENT REFERENCES ---
    public Health Health => _health;
    private Health _health;
    public StatusHandler Status => _status;
    private StatusHandler _status;
    public ZombieNav Nav => _nav;
    private ZombieNav _nav;
    
    // --- TEMPORARY REFERENCES ---
    public DataZombie Data { get; private set; }
    public ZombieAnimator Anim { get; private set; }
    protected HealthFlash Flash;

    // --- BASE PARAMETERS ---
    public float Speed => Status.ModConst(AffectorConstType.Speed, Data.speed);
    public float Damage => Status.ModConst(AffectorConstType.Damage, Data.attackDamage);
    public float AttackRate => Status.ModConst(AffectorConstType.Speed, Data.attackRate);
    public float BaseHealth => Status.ModConst(AffectorConstType.Speed, Data.health);
    public float BaseRange => Status.ModConst(AffectorConstType.Speed, Data.attackRange);
    
    // --- STATE PARAMETERS ---
    public Vector3 Position => Health.Position;
    public Vector3 Velocity => Nav.Velocity;
    public Vector3 MoveDirection => Vector3.Scale(Velocity, new Vector3(1, 0, 1)).normalized;
    public Vector3 TargetDirection => Nav.TargetDirection;
    
    //public float CurrentResist => Status.ModConstant(AffectorConstType.BaseAttackDamage, Data.resistance);
    
    // ------ START METHODS ------
    
    public void Initialize(ManagerZombies manager, int id, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        TryGetComponent(out _health);
        TryGetComponent(out _status);
        TryGetComponent(out _nav);
        if (!Health || !Status || !Nav) {
            enabled = false;
            return;
        }
        
        Id = id;
        Manager = manager;
        gameObject.name = "Zombie " + id;
        Nav.Initialize(this, mainTarget, approachDist, sideTargetMask);
        
        Health.OnDeath += Push;
        Status.EventDynamicModifierApplied += EventDynamicModifierApplied;
    }
    
    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) {
        //Nav.UpdateCustom(deltaTime);
    }

    public void FixedUpdateCustom(float deltaTime, int tick) {
        Nav.FixedUpdateCustom(deltaTime, tick);
        Anim.FixedUpdateCustom(deltaTime, tick);
        Status.FixedUpdateCustom(deltaTime, tick);
    }
    
    // ------ POOLING ------
    
    public void Pop(DataZombie dataZombie, Vector3 spawnPos) {
        if (Id < 0) {
            enabled = false;
            return;
        }

        GameObject visual = Instantiate(dataZombie.visualPrefab, transform);
        if (visual.TryGetComponent(out ZombieAnimator a) && visual.TryGetComponent(out Flash)) Anim = a;
        else {
            enabled = false;
            return;
        }
        
        Data = dataZombie;
        Health.SetMaxHealth(Data.health);
        Health.Reset();
        Anim.Initialize(this);
        Flash.Initialize(Health);
        Nav.Pop(spawnPos);
    }

    /// Returns zombie back to pool with its data erased.
    public void Push() {
        Data = null;
        
        if (Anim) Destroy(Anim.gameObject);
        Anim = null;

        Status.Reset();
        Nav.Reset();
        Manager.Push(this);
    }
    
    public void SetActive(bool active) => gameObject.SetActive(active);

    private void EventDynamicModifierApplied(StatusModifiersList modifiers) {
        float healthModifier = modifiers.GetDynamicModifier(AffectorDynamicType.CurrentHealth);
        
        if (!Mathf.Approximately(healthModifier, 0))
            Health.DealDamage(healthModifier);
    }
}