using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoSingleton<ParticleManager>
{
    public ParticleEffectPool bloodEffectPool;
    //public ParticleEffectPool attachedEffectPool;
    public List<ParticleEffectPool> attachedEffectPools;

    public ParticleParent tapEffect;
    public ParticleParent swipeEffect;
    public ParticleParent shakeEffect;

    public GameObject slowEffect;
    public GameObject poisonEffect;

    public float currentTime;

    private void Start()
    {
        SetTapEffect(ManagerWeapon.Inst.tapWeapons[0].Data.particleEffect);
        SetSwipeEffect(ManagerWeapon.Inst.swipeWeapons[0].Data.particleEffect);
        SetShakeEffect(ManagerWeapon.Inst.shakeWeapons[0].Data.particleEffect);
    }

    private void Update()
    {
        currentTime = Time.time;
    }

    public int CreatePool(GameObject particlePrefab)
    {
        Debug.Log(attachedEffectPools.Count);
        for (int i = 0; i < attachedEffectPools.Count; i++)
        {
            Debug.Log("compared pool name: " + attachedEffectPools[i].poolParticle.name
                + " comparer pool name: " + particlePrefab.name);
            if (attachedEffectPools[i].poolParticle.name == particlePrefab.name)
                return i;
        }
        ParticleEffectPool pool = gameObject.AddComponent<ParticleEffectPool>();
        pool.SetParticle(particlePrefab);
        pool.isAttacherPool = true;
        attachedEffectPools.Add(pool);
        pool.poolIndex = attachedEffectPools.Count - 1;
        return pool.poolIndex;
    }

    public void ReturnParticleToPool(ParticleParent particle, int origin)
    {
        attachedEffectPools[origin].ReturnParticleToPool(particle);
    }

    private void SetTapEffect(GameObject effect)
    {
        if (effect != null)
            tapEffect = Instantiate(effect).GetComponent<ParticleParent>();
    }
    private void SetSwipeEffect(GameObject effect)
    {
        if (effect != null)
            swipeEffect = Instantiate(effect).GetComponent<ParticleParent>();
    }
    private void SetShakeEffect(GameObject effect)
    {
        if (effect != null)
            shakeEffect = Instantiate(effect).GetComponent<ParticleParent>();
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
        Debug.Log("Shake effect played");
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
    }

    protected override void OnDestroyed(bool isDeletedInstance)
    {

    }
}
