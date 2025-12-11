using UnityEngine;
using UnityEngine.VFX;

public class ParticleParent : MonoBehaviour
{
    public VisualEffect[] effects;
    //public bool attached = false;
    public Transform objectToFollow;
    public float endTime = -1;
    public int poolIndex = -1;

    public bool IsPlaying
    {
        get
        {
            foreach (var effect in effects)
            {
                if (effect.HasAnySystemAwake())
                {
                    return true;
                }
            }
            return false;

        }
    }

    public void SetEndTime(float duration)
    {
        endTime = Time.time + duration;
    }

    public void SetObjectToFollow(Transform objectToFollow)
    {
        this.objectToFollow = objectToFollow;
    }

    public void ReturnToPool()
    {
        ParticleManager.Inst.ReturnParticleToPool(this, poolIndex);
    }

    public void Reset()
    {
        SetObjectToFollow(null);
        endTime = -1;
        transform.SetParent(null);
        transform.position = Vector3.zero;
        Stop();
        //transform.position -= new Vector3(0, 10, 0);
    }

    public void Play()
    {
        foreach (var effect in effects)
        {
            effect.Play();
        }
    }

    public void Stop()
    {
        foreach (var effect in effects)
        {
            effect.Stop();
        }
    }
}

