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

    public List<ParticleParent> ActiveParticles = new List<ParticleParent>();
    public List<ParticleParent> InactiveParticles = new List<ParticleParent>();

    public float timeBetweenCleanups = 0.1f;

    public bool isAttacherPool = false;
    public int poolIndex = 0;

    public void StartPool()
    {
        CreateNewParticle(5);
        StartPoolProcesses();
    }

    private void OnDisable()
    {
        StopPoolProcesses();
    }

    public void SetParticle(GameObject particle)
    {
        poolParticle = particle;
    }

    public void ActivateParticle(Vector3 pos)
    {
        // get a particle
        var particle = PopParticle();
        // place it at the position
        particle.transform.position = pos;
        // play it
        particle.Play();
        // add particle to active particle list
        ActiveParticles.Add(particle);
    }

    public void ActivateParticle(Transform objectToFollow, float duration)
    {
        // get a particle
        var particle = PopParticle();
        // assign the object to follow
        particle.objectToFollow = objectToFollow;
        particle.transform.SetParent(objectToFollow, false);

        // play it
        particle.Play();
        // set the end time
        particle.SetEndTime(duration);
        // add particle to active particle list
        ActiveParticles.Add(particle);
    }

    public ParticleParent PopParticle()
    {
        if (InactiveParticles.Count <= 0)
        {
            CreateNewParticle(5);
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
            temp.poolIndex = poolIndex;
            InactiveParticles.Add(temp);
        }
    }

    public void ReturnParticleToPool(ParticleParent particle)
    {
        //Debug.Log("returning particle to pool");
        ActiveParticles.Remove(particle);
        InactiveParticles.Add(particle);
        particle.Reset();
    }

    public void StartPoolProcesses()
    {
        //Debug.Log("isAttacherPool: " + isAttacherPool);
        InvokeRepeating(nameof(CleanUpParticles), 0.5f, timeBetweenCleanups);
    }

    public void UpdateParticlePositions()
    {
        //Debug.Log("updating positions");
        foreach (var particle in ActiveParticles)
        {
            if (particle.objectToFollow != null)
            {
                particle.transform.position = particle.objectToFollow.position;
            }
        }
    }
    
    public void CleanUpParticles()
    {
        float currentTime = Time.time;
        if (ActiveParticles.Count > 0)
        {
            for (int i = 0; i < ActiveParticles.Count && ActiveParticles.Count > 0; i++)
            {
                ParticleParent particle = ActiveParticles[i];
                if (particle != null)
                {
                    if (particle.endTime == -1 && !particle.IsPlaying
                        || particle.endTime <= Time.time
                        || particle.objectToFollow == null && particle.endTime != -1)
                    {
                        ReturnParticleToPool(particle);
                        i--;
                    }
                    //else if (particle.endTime <= Time.time)
                    //{
                    //    ReturnParticleToPool(particle);
                    //    i--;
                    //}
                }
                else
                {
                    //Debug.LogError("particle at index " + i + " is null. Check that particles are not being destroyed before returning");
                    ActiveParticles.RemoveAt(i);
                    i--;
                }
            }
        }
    }

    public void StopPoolProcesses()
    {
        CancelInvoke();
    }


}
