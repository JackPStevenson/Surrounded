using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class WeaponBase : MonoBehaviour {
    public event Action<Health[]> EventOnHit;
    public event Action<Health[]> EventOnKill;
    public event Action<Vector3> EventOnTap;
    public event Action<Vector3[]> EventOnSwipe;
    public event Action EventOnShake;
    protected void EventTap(Vector3 pos) => EventOnTap?.Invoke(pos);
    protected void EventSwipe(Vector3[] points) => EventOnSwipe?.Invoke(points);
    protected void EventShake() => EventOnShake?.Invoke();

    // --- DATA ENTRY ---
    [Header("Weapon Data")]
    public DataWeapon weaponData;
    // private WeaponDataTypes _weaponData; // Make this match your weapon's actual type.

    // --- DATA REFERENCES ---
    public LayerMask HitMask => weaponData.hitMask;
    public float Damage => PlayerCore.Inst.Status.ModConst(AffectorConstType.Damage, weaponData.damage);
    public float Range => PlayerCore.Inst.Status.ModConst(AffectorConstType.Range, weaponData.range);
    public int Penetration => Mathf.CeilToInt(PlayerCore.Inst.Status.ModConst(AffectorConstType.Penetration, weaponData.penetration));

    public float EnergyRegenRate => PlayerCore.Inst.Status.ModConst(AffectorConstType.EnergyRegen, weaponData.energyRegenRate);
    public float EnergyCost => weaponData.energyCost / PlayerCore.Inst.Status.ModConst(AffectorConstType.MaxEnergy, 1);

    // --- CURRENT STATE ---
    public bool Active { get; private set; }
    public float CurrentEnergy { get; private set; }
    protected bool AttackUsed = false;

    // ------ UPDATE FUNCTIONS ------

    void Awake() {
        if (!weaponData || !TryParseData()) {
            print(gameObject.name);
            Debug.LogError("Unable to parse data to weapon's type.");
            enabled = false;
            return;
        }

        OnAwake();
    }

    protected abstract bool TryParseData();
    protected virtual void OnAwake() { }

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

    protected void PerformHit(Health[] comps, float damage) {
        List<Health> killedComps = new List<Health>();
        foreach (Health h in comps) if (h.DealDamage(damage) <= 0) killedComps.Add(h);
        EventOnHit?.Invoke(comps);
        EventOnKill?.Invoke(killedComps.ToArray());
    }
}