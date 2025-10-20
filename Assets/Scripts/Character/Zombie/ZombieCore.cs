using System;
using UnityEngine;

[RequireComponent(typeof(Health))] [RequireComponent(typeof(StatusHandling))] [RequireComponent(typeof(ZombieNav))]
public class ZombieCore : MonoBehaviour, IUpdateCustom {
    protected ZombiesManager Manager;
    
    public int Id { get; private set; } = -1;
    
    // --- PERMANENT REFERENCES ---
    public Health Health { get; private set; }
    public StatusHandling Status { get; private set; }
    public ZombieNav Nav { get; private set; }
    
    // --- TEMPORARY REFERENCES ---
    public ZombieData Data { get; private set; }
    public ZombieAnimator Anim { get; private set; }
    protected HealthFlash Flash;

    // --- BASE PARAMETERS ---
    public float BaseTargetSpeed => Data.speed;
    public float BaseDamage => Data.attackDamage;
    public float BaseAttackRate => Data.attackRate;
    public float BaseHealth => Data.health;
    public float BaseRange => Data.attackRange;
    
    // --- STATE PARAMETERS ---
    public Vector3 Position => Health.Position;
    public Vector3 Velocity => Nav.Velocity;
    public float Speed => Nav.Velocity.magnitude;
    public Vector3 MoveDirection => Vector3.Scale(Velocity, new Vector3(1, 0, 1)).normalized;
    public Vector3 TargetDirection => Nav.TargetDirection;
    
    public float CurrentTargetSpeed => BaseTargetSpeed * Status.SpeedMod;
    public float CurrentDamage => Data.attackDamage * Status.DamageMod;
    public float CurrentResist => Status.ResistMod;
    
    // ------ START METHODS ------
    
    public void Initialize(ZombiesManager manager, int id, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        if (TryGetComponent(out Health h)) Health = h;
        if (TryGetComponent(out StatusHandling s)) Status = s;
        if (TryGetComponent(out ZombieNav n)) Nav = n;
        if (!Health || !Status || !Nav) {
            enabled = false;
            return;
        }
        
        Id = id;
        Manager = manager;
        gameObject.name = "Zombie " + id;
        Nav.Initialize(this, mainTarget, approachDist, sideTargetMask);
        Health.OnDeath += Push;
        Status.OnModHealthOverTime += Health.ModHealthNoReturn;
    }
    
    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) {
        //Nav.UpdateCustom(deltaTime);
    }

    public void FixedUpdateCustom(int tick, float deltaTime) {
        Nav.FixedUpdateCustom(tick, deltaTime);
        Anim.FixedUpdateCustom(tick, deltaTime);
        Status.FixedUpdateCustom(tick, deltaTime);
    }
    
    // ------ POOLING ------
    
    public void Pop(ZombieData zombieData, Vector3 spawnPos) {
        if (Id < 0) {
            enabled = false;
            return;
        }

        GameObject visual = Instantiate(zombieData.visualPrefab, transform);
        if (visual.TryGetComponent(out ZombieAnimator a) && visual.TryGetComponent(out Flash)) Anim = a;
        else {
            enabled = false;
            return;
        }
        
        Data = zombieData;
        Health.SetMaxHealth(Data.health);
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
}