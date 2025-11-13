using UnityEngine;

public class AnimLayerInfluenceTaper : StateMachineBehaviour {
    [Header("Influence")]
    public AnimationCurve influenceCurve;
    
    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetLayerWeight(layerIndex, influenceCurve.Evaluate(Mathf.Clamp01(stateInfo.normalizedTime)));
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
        animator.SetLayerWeight(layerIndex, influenceCurve.Evaluate(1));
    }
}