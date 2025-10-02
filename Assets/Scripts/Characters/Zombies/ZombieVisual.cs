using UnityEngine;

public class ZombieVisual : MonoBehaviour {
    private readonly static int MoveSpeed = Animator.StringToHash("MoveSpeed");
    private readonly static int Attack = Animator.StringToHash("OnAttack");
    private readonly static int Death = Animator.StringToHash("OnDeath");
    private readonly static int Damaged = Animator.StringToHash("OnDamaged");

    private ZombieBase _zombie;
    
    [Header("References")]
    public Animator zombieAnimator;
    public MeshRenderer[] renderers;
    public SkinnedMeshRenderer[] skinnedRenderers;

    void Awake() {
        
    }
    
    public void SetZombie(ZombieBase newZombie) {
        _zombie = newZombie;
        _zombie.OnDeath += OnDeath;
        _zombie.OnDamaged += OnDamaged;
        _zombie.OnAttack += OnAttack;
    }
    
    void Update()
    {
        zombieAnimator.SetFloat(MoveSpeed, _zombie.GetCurrentSpeed() / _zombie.GetMaxSpeed());
    }
    
    void OnAttack() {
        zombieAnimator.SetTrigger(Attack);
    }

    void OnDeath() {
        zombieAnimator.SetTrigger(Death);
    }
    
    void OnDamaged(float damageTaken, float healthLeft) {
        // Entirely optional.
        zombieAnimator.SetTrigger(Damaged);
    }
}
