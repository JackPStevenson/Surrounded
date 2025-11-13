using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

[RequireComponent(typeof(HealthFlash))]
public class ZombieAnimator : MonoBehaviour, IUpdateCustom {
    private readonly static int UseRunningAnim = Animator.StringToHash("UseRunningAnim");
    private readonly static int MoveState = Animator.StringToHash("MoveState");
    private readonly static int MoveSpeedScalar = Animator.StringToHash("MoveSpeedScalar");
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
    private float _moveState = 0;

    // ------ START METHODS ------

    public void Initialize(ZombieCore core) {
        Core = core;
        
        foreach (Transform child in transform) if (child.TryGetComponent(out Anim)) break;
        TryGetComponent(out Flash);
        
        Flash.Initialize(core.Health);
        core.Nav.OnAttack += OnAttack;
        core.Health.EventHealthChange += EventHealthChange;
        core.Health.EventDeath += EventDeath;
    }

    // ------ UPDATE METHODS ------

    public void UpdateCustom(float deltaTime) { }

    public void FixedUpdateCustom(float deltaTime, int tick) {
        // Update zombie visual's direction based on direction its moving.
        Vector3 targetDir = Vector3.Scale(Core.TargetDirection, new Vector3(1, 0, 1)).normalized;
        if (targetDir.magnitude > 0) transform.rotation = Quaternion.LookRotation(targetDir);

        // Update animator move parameter based on zombie's current velocity. Smoothly transition value to prevent choppiness.
        //float targetMoveState = Mathf.Clamp01(Core.Velocity.magnitude);
        float targetMoveState = Core.Nav.NavState is NavState.Moving ? 1 : 0;
        _moveState = Common.SmoothLerp(_moveState, targetMoveState, AnimSmoothing, Time.fixedDeltaTime);

        Anim.SetFloat(UseRunningAnim, useRunningAnimation ? 1 : 0);
        Anim.SetFloat(MoveState, _moveState);

        float speedDelta = Core.Velocity.magnitude * 0.9f;
        Anim.SetFloat(MoveSpeedScalar, speedDelta);
    }

    // ------ EVENT METHODS ------

    void OnAttack() => Anim.SetTrigger(Attack);
    void EventHealthChange(float healthChange) {
        if(healthChange < 0) Anim.SetTrigger(Damaged);
    }
    void EventDeath() => Anim.SetTrigger(Death);

    private void OnDestroy() {
        if(!Core) return;
        Core.Nav.OnAttack -= OnAttack;
        Core.Health.EventHealthChange -= EventHealthChange;
        Core.Health.EventDeath -= EventDeath;
    }
}