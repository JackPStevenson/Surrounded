using UnityEngine;
using UnityEngine.VFX;

public class ParticleParent : MonoBehaviour
{
    public VisualEffect[] effects;
    public bool attached = false;
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

    public void Play()
    {
        foreach (var effect in effects)
        {
            //Debug.Log("Playing");
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

