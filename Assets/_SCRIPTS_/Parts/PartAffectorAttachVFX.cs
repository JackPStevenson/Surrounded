using UnityEngine;

public class PartAffectorAttachVFX : PartAffector
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
        ParticleParent particle = comp.GetComponentInChildren<ParticleParent>();
        ParticleEffectPool pool = ParticleManager.Inst.attachedEffectPools[indexOfPool];
        if (particle != null)
        {
            particle.SetEndTime(effect.duration);
        }
        else
        {
            pool.ActivateParticle(comp.transform, effect.duration);
        }

    }

    public void AffectComp(Health comp)
    {
        OnCompAffect(comp);
    }
}
