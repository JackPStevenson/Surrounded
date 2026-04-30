using System.Collections;
using UnityEngine;

[RequireComponent(typeof(HealthFlash))]
public class ZombieAnimator : MonoBehaviour, IUpdateCustom {
    private const float corpseLifetime = 5;
    
    private readonly static int UseRunningAnim = Animator.StringToHash("UseRunningAnim");
    private readonly static int MoveState = Animator.StringToHash("MoveState");
    private readonly static int MoveSpeedScalar = Animator.StringToHash("MoveSpeedScalar");
    private readonly static int Attack = Animator.StringToHash("OnAttack");
    private readonly static int Death = Animator.StringToHash("OnDeath");
    private readonly static int Damaged = Animator.StringToHash("OnDamaged");
    private readonly static int AttackSpeedScalar = Animator.StringToHash("AttackSpeedScalar");
    private const float AnimSmoothing = 0.000001f;
    
    public bool useRunningAnimation = false;

    // --- PERMANENT REFERENCES ---
    protected ZombieCore Core;
    protected Animator Anim;
    protected HealthFlash Flash;
    protected CharacterVisualDamage VisualDamage;

    // --- STATE PARAMETERS ---
    private float _moveState = 0;

    // ------ START METHODS ------

    public void Initialize(ZombieCore core) {
        Core = core;

        foreach (Transform child in transform)
            if (child.TryGetComponent(out Anim)) break;
        
        TryGetComponent(out Flash);
        TryGetComponent(out VisualDamage);

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

        // Record current state parameters.
        bool isMoving = Core.IsMoving;
        float moveStateTarget = isMoving ? 1 : 0;
        float moveSpeedScalar = Mathf.Max(isMoving ? Core.CurrentSpeed : Core.AttackSpeed);

        // Update animator parameters.
        _moveState = Common.SmoothLerp(_moveState, moveStateTarget, AnimSmoothing, Time.fixedDeltaTime);

        Anim.SetFloat(MoveState, _moveState);

        Anim.SetFloat(MoveSpeedScalar, moveSpeedScalar);
        Anim.SetFloat(UseRunningAnim, useRunningAnimation ? 1 : 0);

        Anim.SetFloat(AttackSpeedScalar, Core.AttackSpeed);
        
        VisualDamage?.FixedUpdateCustom(deltaTime, tick);
    }

    // ------ EVENT METHODS ------

    void OnAttack() {
        Anim.SetTrigger(Attack);
        Core._audioPlayer.PlayAttackSound();
    }

    void EventHealthChange(float healthChange) {
        if (healthChange < 0) Anim.SetTrigger(Damaged);
        VisualDamage?.ChangeHealth(Core.Health.HealthCurrent / Core.MaxHealth);
    }

    void EventDeath(string deathSource = "") {
        if (Core) {
            Core.Nav.OnAttack -= OnAttack;
            Core.Health.EventHealthChange -= EventHealthChange;
            Core.Health.EventDeath -= EventDeath;
        }
        
        Anim.SetTrigger(Death);
        transform.SetParent(null);
        StartCoroutine(DeathCoroutine());
    }

    private IEnumerator DeathCoroutine() {
        yield return new WaitForSeconds(corpseLifetime);
        Destroy(gameObject);
    }
}