using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(HealthFlash))]
public class ZombieAnimator : MonoBehaviour, IUpdateCustom {
    private readonly static int UseRunningAnim = Animator.StringToHash("UseRunningAnim");
    private readonly static int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly static int Attack = Animator.StringToHash("OnAttack");
    private readonly static int Death = Animator.StringToHash("OnDeath");
    private readonly static int Damaged = Animator.StringToHash("OnDamaged");
    private const float AnimSmoothing = 0.000001f;

    public bool useRunningAnimation = false;
    
    // --- PERMANENT REFERENCES ---
    protected ZombieCore Core;
    protected Animator Anim;
    protected HealthFlash Flash;
    
    // --- STATE PARAMETERS ---
    private float _animMoveVar = 0;

    // ------ START METHODS ------

    public void Initialize(ZombieCore core) {
        Core = core;
        
        foreach (Transform child in transform)
            if (child.TryGetComponent(out Anim))
                break;
        if (!Anim || !TryGetComponent(out Flash)) {
            enabled = false;
            return;
        }
        
        Flash.Initialize(core.Health);
        core.Nav.OnAttack += OnAttack;
        core.Health.OnModHealth += OnModHealth;
        core.Health.OnDeath += OnDeath;
    }

    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) { }

    public void FixedUpdateCustom(float deltaTime, int tick) {
        // Update zombie visual's direction based on direction its moving.
        Vector3 targetDir = Core.TargetDirection;
        if (targetDir.magnitude > 0) transform.rotation = Quaternion.LookRotation(targetDir);

        // Update animator move parameter based on zombie's current velocity. Smoothly transition value to prevent choppiness.
        float targetMoveSpeed = Core.Speed / Mathf.Max(Core.BaseTargetSpeed, 0.1f);
        _animMoveVar = Common.SmoothLerp(_animMoveVar, targetMoveSpeed, AnimSmoothing, Time.fixedDeltaTime);
        
        Anim.SetFloat(UseRunningAnim, useRunningAnimation ? 1 : 0);
        Anim.SetFloat(MoveSpeed, _animMoveVar);
    }

    // ------ EVENT METHODS ------

    void OnAttack() => Anim.SetTrigger(Attack);
    void OnModHealth(float damageTaken) { /*zombieAnimator.SetTrigger(Damaged); */ }
    void OnDeath() => Anim.SetTrigger(Death);

    private void OnDestroy() {
        if(!Core) return;
        Core.Nav.OnAttack -= OnAttack;
        Core.Health.OnModHealth -= OnModHealth;
        Core.Health.OnDeath -= OnDeath;
    }
}