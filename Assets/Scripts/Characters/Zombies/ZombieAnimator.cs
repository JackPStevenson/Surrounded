using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAnimator : MonoBehaviour {
    private readonly static int MoveSpeedHash = Animator.StringToHash("MoveSpeed");
    private readonly static int AttackHash = Animator.StringToHash("Attack");
    private readonly static int DeathHash = Animator.StringToHash("Death");

    private Animator _animator;

    void Awake() {
        _animator = GetComponent<Animator>();
    }

    public void SetMoveSpeed(float speed) {
        _animator.SetFloat(MoveSpeedHash, speed);
    }

    public void Attack() {
        _animator.SetTrigger(AttackHash);
    }
    
    public void Die() {
        _animator.SetTrigger(DeathHash);
    }

    public void AttackHit() {
        print("Hit");
    }
}
