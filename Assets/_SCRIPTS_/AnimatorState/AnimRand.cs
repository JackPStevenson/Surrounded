using UnityEngine;
using UnityEngine.Serialization;

public abstract class AnimRand : StateMachineBehaviour {
    [Header("General")]
    public string variableName;
    protected int VariableHash { get { if (_varHash <= -99999) _varHash = Animator.StringToHash(variableName); return _varHash; } }
    private int _varHash = -999999;
    
    // Called when a transition starts and the state machine starts to evaluate this state.
    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
}