using System;
using UnityEngine;

public class ZombieVisual : MonoBehaviour {
    private readonly static int LastDamageFlash = Shader.PropertyToID("_Last_Damage_Flash");
    private readonly static int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly static int Attack = Animator.StringToHash("OnAttack");
    private readonly static int Death = Animator.StringToHash("OnDeath");
    private readonly static int Damaged = Animator.StringToHash("OnDamaged");
    private const float AnimSmoothing = 0.000001f;

    private ZombieBase _zombie;

    [Header("References")]
    public Animator zombieAnimator;
    public MeshRenderer[] renderers;
    public SkinnedMeshRenderer[] skinnedRenderers;

    private float _moveSpeed = 0;

    // ------ START METHODS ------

    public void Initialize(ZombieBase newZombie) {
        _zombie = newZombie;
        _zombie.OnDeath += OnDeath;
        _zombie.OnDamaged += OnDamaged;
        _zombie.OnAttack += OnAttack;
    }

    // ------ UPDATE METHODS ------

    public void FixedUpdateLoop() {
        Vector3 velocity = _zombie.GetVelocity();
        
        // Update zombie visual's direction based on direction its moving.
        Vector3 targetDir = Vector3.Scale(velocity, new Vector3(1, 0, 1)).normalized;
        if(targetDir.magnitude > 0.01f) transform.rotation = Quaternion.LookRotation(targetDir);
        
        // Update move speed parameter based on zombie's current velocity. Smoothly transition value to prevent choppiness.
        float targetMoveSpeed = velocity.magnitude / Mathf.Max(_zombie.GetMaxSpeed(), 0.001f);
        _moveSpeed = Common.SmoothLerp(_moveSpeed, targetMoveSpeed, AnimSmoothing, Time.fixedDeltaTime);
        zombieAnimator.SetFloat(MoveSpeed, _moveSpeed);
    }

    // ------ EVENT METHODS ------

    void OnAttack() {
        zombieAnimator.SetTrigger(Attack);
    }

    void OnDeath() {
        zombieAnimator.SetTrigger(Death);
    }

    void OnDamaged(float damageTaken, float healthLeft) {
        //zombieAnimator.SetTrigger(Damaged);
        FlashDamage();
    }

    private void OnDestroy() {
        _zombie.OnDeath -= OnDeath;
        _zombie.OnDamaged -= OnDamaged;
        _zombie.OnAttack -= OnAttack;
    }

    // ------ VISUAL METHODS ------
    
    private void FlashDamage() {
        foreach (MeshRenderer r in renderers) r?.material.SetFloat(LastDamageFlash, Time.time);
        foreach (SkinnedMeshRenderer r in skinnedRenderers) r?.material.SetFloat(LastDamageFlash, Time.time);
    }
}