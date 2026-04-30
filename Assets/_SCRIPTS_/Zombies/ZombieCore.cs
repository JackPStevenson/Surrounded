using UnityEngine;

[RequireComponent(typeof(ZombieNav))]
public class ZombieCore : CharacterCore {
    public int Id { get; private set; } = -1;
    
    // --- PERMANENT REFERENCES ---
    public ZombieNav Nav => _nav;
    private ZombieNav _nav;
    
    // --- TEMPORARY REFERENCES ---
    public DataZombie Data => _data;
    private DataZombie _data;
    public ZombieAnimator Anim => _anim;
    private ZombieAnimator _anim;

    // --- BASE PARAMETERS ---
    public float Speed => Status.ModConst(AffectorConstType.Speed, _data.speed);
    public float Damage => Status.ModConst(AffectorConstType.Damage, _data.attackDamage);
    public float AttackSpeed => Status.ModConst(AffectorConstType.AttackSpeed, _data.attackSpeed);
    public float AttackInterval => 1f / Mathf.Max(AttackSpeed, 0.000001f);
    public float MaxHealth => Status.ModConst(AffectorConstType.MaxHealth, _data.health);
    public float Range => Status.ModConst(AffectorConstType.Range, _data.attackRange);
    
    // --- STATE PARAMETERS ---
    public Vector3 Velocity => Nav.Velocity;
    public float CurrentSpeed => Nav.Velocity.magnitude;
    public Vector3 TargetDirection => Nav.TargetDirection;
    public bool IsMoving => Nav.NavState is NavState.Moving;
    
    public float CurrentResist => Status.ModConst(AffectorConstType.Resistance, 1);
    
    // ------ START METHODS ------
    
    public void Initialize(int id, Health mainTarget, float approachDist, LayerMask sideTargetMask) {
        Initialize();
        
        Id = id;
        gameObject.name = "Zombie " + id;
        
        TryGetComponent(out _nav);
        Nav.Initialize(this, mainTarget, approachDist, sideTargetMask);
    }
    
    // ------ UPDATE METHODS ------

    protected override void OnFixedUpdateCustom(float deltaTime, int tick) {
        Nav.FixedUpdateCustom(deltaTime, tick);
        Anim.FixedUpdateCustom(deltaTime, tick);
    }
    
    // ------ POOLING ------

    public void Spawn(DataZombie dataZombie, Vector3 spawnPos) {
        if (Id < 0) return;
        
        _data = dataZombie;

        GameObject go = Instantiate(dataZombie.visualPrefab, transform);
        go.TryGetComponent(out _anim);
        
        Nav.Spawn(spawnPos);
        Activate();
    }

    protected override void OnActivate() {
        Health.SetMaxHealth(_data.health);
        Anim.Initialize(this);
        gameObject.SetActive(true);
    }

    /// Returns zombie back to pool with its data erased.
    protected override void OnDeactivate() {
        Nav.Reset();
        _data = null;

        _anim = null;
        for(int i = transform.childCount - 1; i >= 0; i--)
            Destroy(transform.GetChild(i).gameObject);
        
        _anim = null;
    }
}