using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    private ParticleEffects effectsBundle;
    public ParticleEffectPool bloodEffectPool;
    public ParticleEffectPool slowEffectPool;


    private ParticleParent tapEffect;
    private ParticleParent swipeEffect;
    public ParticleParent shakeEffect;

    private void Start()
    {
        //SetTapEffect(ManagerWeapon.Inst.tapWeapons[0].Data.particleEffect);
        //SetSwipeEffect(ManagerWeapon.Inst.swipeWeapons[0].Data.particleEffect);
        //SetShakeEffect(ManagerWeapon.Inst.shakeWeapons[0].Data.particleEffect);
    }

    private void SetTapEffect(string effect)
    {
        var particleObject = effectsBundle.GetParticle(effect);
        if (particleObject != null)
        {
            tapEffect = Instantiate(particleObject.GetComponent<ParticleParent>());
        }
    }
    private void SetSwipeEffect(string effect)
    {
        var particleObject = effectsBundle.GetParticle(effect);
        if (particleObject != null)
        {
            swipeEffect = Instantiate(particleObject.GetComponent<ParticleParent>());
        }
    }
    private void SetShakeEffect(string effect)
    {
        var particleObject = effectsBundle.GetParticle(effect);
        if (particleObject != null)
        {
            shakeEffect = Instantiate(particleObject.GetComponent<ParticleParent>());
        }
    }

    public void AttachParticle(ParticleParent effect, Transform parent)
    {
        effect.transform.SetParent(parent, false);
        effect.attached = true;
    }

    public void DettachParticle(ParticleParent effect)
    {
        effect.attached = false;
        effect.transform.SetParent(null);
    }

    public void PlayTapEffect(Vector3 pos)
    {
        tapEffect.transform.position = pos;
        tapEffect.Play();
    }

    public void StopTapEffect()
    {
        tapEffect.Stop();
    }

    public void PlaySwipeEffect(Vector3[] pos)
    {
        swipeEffect.Play();
    }

    public void StopSwipeEffect()
    {
        swipeEffect.Stop();
    }

    public void PlayShakeEffect()
    {
        shakeEffect.Play();
    }

    public void StopShakeEffect()
    {
        shakeEffect.Stop();
    }

    protected override void OnAwake()
    {

    }

    public void PlayBloodEffect(Vector3 pos)
    {
        bloodEffectPool.ActivateParticle(pos);
        Debug.Log("Played in manager");
    }
}
