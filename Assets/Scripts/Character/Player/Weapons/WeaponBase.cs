using System;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour {
    public event Action<Health[]> EventOnHit;
    protected void OnHit(Health[] h) => EventOnHit?.Invoke(h);
    
    // --- DATA ENTRY ---
    [Header("Weapon Data")]
    public WeaponDataBase weaponData;
    // private WeaponDataTypes _weaponData; // Make this match your weapon's actual type.
    
    // --- DATA REFERENCES ---
    public LayerMask HitMask => weaponData.hitMask;
    public float Damage => weaponData.damage;
    public float Range => weaponData.range;
    public int Penetration => weaponData.penetration;
    
    public float EnergyRegenRate => weaponData.energyRegenRate;
    public float EnergyCost => weaponData.energyCost;
    
    // --- CURRENT STATE ---
    public bool Active { get; private set; }
    public float CurrentEnergy { get; private set; }
    protected bool AttackUsed = false;
    
    // ------ UPDATE FUNCTIONS ------

    void Start() {
        if (!weaponData || !TryParseData()) {
            Debug.LogError("Unable to parse data to weapon's type.");
            enabled = false;
            return;
        }
        
        OnStart();
    }

    protected abstract bool TryParseData();
    protected virtual void OnStart() {}
    
    // ------ UPDATE FUNCTIONS ------
    
    void FixedUpdate() {
        CurrentEnergy = Mathf.Clamp01(CurrentEnergy + (Time.fixedDeltaTime * EnergyRegenRate));
        
        OnFixedUpdate(Time.fixedDeltaTime);
    }

    protected virtual void OnFixedUpdate(float deltaTime) { }
    
    // ------ EVENT FUNCTIONS ------
    
    public void OnTouchPress(Vector3 pos) {
        TouchPressAction(pos);
        
    }
    
    public void OnSwipe(Vector3 pos) {
        SwipeAction(pos);
    }
    
    public void OnTouchRelease(Vector3 pos) {
        TouchReleaseAction(pos);
    }

    public void OnShake() {
        ShakeAction();
    }

    // ------ ACTION FUNCTIONS ------

    protected virtual void TouchPressAction(Vector3 pos) { }
    protected virtual void SwipeAction(Vector3 pos) { }
    protected virtual void TouchReleaseAction(Vector3 pos) { }
    protected virtual void ShakeAction() { }
    
    protected virtual void OnToggleWeapon(bool isEnabled) { }

    
    // ------ HELPER FUNCTIONS ------

    public void ToggleWeapon(bool isEnabled) {
        if (Active == isEnabled) return;
        Active = isEnabled;
        OnToggleWeapon(Active);
    }
    
    /// Returns whether weapon has at least given amount of energy.
    public bool HasEnoughEnergy() => HasEnoughEnergy(EnergyCost);
    public bool HasEnoughEnergy(float energyNeeded) => CurrentEnergy >= energyNeeded;

    /// Tries to consume given amount of energy from current energy. Returns whether consumption was successful.
    public bool TryUseEnergy(float energyNeeded) {
        if (!HasEnoughEnergy(energyNeeded)) return false;

        ModifyEnergy(-energyNeeded);
        return true;
    }
    
    /// Directly modifies energy value. For most cases, use TryUseEnergy instead.
    public void ModifyEnergy(float value) => CurrentEnergy = Mathf.Clamp01(CurrentEnergy + value);
}