using UnityEngine;
using UnityEngine.Serialization;

public class AnimRandFloat : AnimRand {
    [Header("Float")]
    public int domainSize = 2;
    public Vector2 range;
    
    // Called when a transition starts and the state machine starts to evaluate this state.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        float interp = Random.Range(0, domainSize) / (domainSize - 1.0f);
        animator.SetFloat(VariableHash, Mathf.Lerp(range.x, range.y, interp));
    }
}