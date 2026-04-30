using UnityEngine;

[CreateAssetMenu(fileName = "ParticleEffects", menuName = "Scriptable Objects/ParticleEffects")]
public class ParticleEffects : ScriptableObject
{
    public GameObject[] ParticleEffectBundle;

    public GameObject GetParticle(string particleName)
    {
        foreach (var particle in ParticleEffectBundle)
        {
            if (particle.name == particleName)
            {
                return particle;
            }
        }
        Debug.LogError("Particle of " + particleName + " does not exist");
        return null;
    }
}
