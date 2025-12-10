using System.Collections.Generic;
using UnityEngine;

public class ParticleEffectPool : MonoBehaviour
{
    public GameObject poolParticle;

    public int TotalParticles
    {
        get
        {
            return ActiveParticles.Count + InactiveParticles.Count;
        }
    }

    public List<ParticleParent> ActiveParticles;
    public List<ParticleParent> InactiveParticles;

    public float timeBetweenCleanups = 0.1f;

    private void Start()
    {
        StartCleaning();
        CreateNewParticle(5);
    }

    private void OnDisable()
    {
        StopCleaning();
    }

    public void SetParticle(GameObject particle)
    {
        poolParticle = particle;
    }

    public void ActivateParticle(Vector3 pos)
    {
        //Debug.Log("activateParticle " + pos);
        // get a particle
        var particle = PopParticle();
        // place it at the position
        particle.transform.position = pos;
        // play it
        particle.Play();
        // add particle to active particle list
        ActiveParticles.Add(particle);
    }

    public void ActivateParticle(Transform parent)
    {
        // get a particle
        var particle = PopParticle();
        // place it at the position
        particle.transform.SetParent(parent, false);
        particle.attached = true;
        // play it
        particle.Play();
    }

    public ParticleParent PopParticle()
    {
        if (InactiveParticles.Count <= 0)
        {
            CreateNewParticle(ActiveParticles.Count);
        }
        ParticleParent temp = InactiveParticles[0];
        InactiveParticles.RemoveAt(0);
        return temp;
    }

    public void CreateNewParticle(int count)
    {
        for (int i = 0; i < count && poolParticle != null; i++)
        {
            var temp = Instantiate(poolParticle).GetComponent<ParticleParent>();
            temp.Stop();
            InactiveParticles.Add(temp);
        }
    }

    public void ReturnParticleToPool(ParticleParent particle)
    {
        ActiveParticles.Remove(particle);
        InactiveParticles.Add(particle);
        if (particle.attached)
        {
            particle.transform.SetParent(null);
        }
    }

    public void StartCleaning()
    {
        InvokeRepeating(nameof(CleanUpParticles), 0.5f, timeBetweenCleanups);
    }

    public void StopCleaning()
    {
        CancelInvoke();
    }

    public void CleanUpParticles()
    {
        if (ActiveParticles.Count > 0)
        {
            for (int i = 0; i < ActiveParticles.Count && ActiveParticles.Count > 0; i++)
            {
                ParticleParent particle = ActiveParticles[i];
                if (!particle.IsPlaying)
                {
                    ReturnParticleToPool(particle);
                    i--;
                }
            }
        }
    }
}
