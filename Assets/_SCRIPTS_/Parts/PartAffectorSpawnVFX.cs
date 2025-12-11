using UnityEngine;

public class PartAffectorSpawnVFX : PartAffector
{
    // --- STATUS ---
    [Header("Characteristics")]
    public int indexOfPool = -1;

    [Header("Effect Characteristics")]
    public DataStatusEffect effect;

    // ------ PART FUNCTIONS ------

    private void Start()
    {
        indexOfPool = ParticleManager.Inst.CreatePool(effect.particleEffect);
        ParticleManager.Inst.attachedEffectPools[indexOfPool].StartPool();
    }

    protected override void OnCompAffect(Health comp)
    {
        ParticleManager.Inst.attachedEffectPools[indexOfPool].ActivateParticle(comp.transform.position);
    }

    public void AffectComp(Health comp)
    {
        OnCompAffect(comp);
    }
}
