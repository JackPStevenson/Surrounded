using UnityEngine;
using UnityEngine.Serialization;

public class AnimRandBool : AnimRand {
    [Header("Boolean")]
    [Range(0f, 1f)]
    public float chanceForTrue = 0.5f;
    public bool oneShot = false;
    private bool _performed;
    
    // Called when a transition starts and the state machine starts to evaluate this state.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        if (oneShot && _performed) return;
        animator.SetBool(VariableHash, Random.Range(0.0f, 1.0f) <= chanceForTrue);
        _performed = true;
    }
}