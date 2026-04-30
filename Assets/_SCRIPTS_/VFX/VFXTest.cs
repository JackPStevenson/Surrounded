using UnityEngine;

public class VFXTest : MonoBehaviour
{
    public ParticleParent particle;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnGUI()
    {
        if (GUILayout.Button("Play"))
        {
            particle.Play();
        }
        if (GUILayout.Button("Stop"))
        {
            particle.Stop();
        }
    }
}
