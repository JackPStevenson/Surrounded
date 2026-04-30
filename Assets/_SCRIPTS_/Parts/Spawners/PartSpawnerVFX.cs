using UnityEngine;
using UnityEngine.Events;

public class PartSpawnerVFX : Part
{
    // --- SPAWNING ---
    [Header("Spawning")]
    public GameObject spawnablePrefab;
    private Vector3 _spawnPos;

    public void SetSpawnPos(Vector3 pos) => _spawnPos = pos;

    public void SetSpawnPos(Health comp) => _spawnPos = comp.transform.position;

    // --- EVENTS ---
    [Header("Spawn Events")]
    public UnityEvent<Transform> EventSpawned;

    // ------ PART FUNCTIONS ------

    protected override void InvokeLogic()
    {
        Debug.Log("spawned");
        Transform t = Instantiate(spawnablePrefab, _spawnPos, Quaternion.Euler(0, 0, 0)).transform;
        StartCoroutine(t.GetComponent<ParticleParent>().PlayFor(1.2f));
        EventSpawned.Invoke(t);
    }

    public override void Reset()
    {

    }
}
