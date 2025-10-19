using UnityEngine;
using UnityEngine.Events;

public class PartObjSpawner : PartBase {
    // --- SPAWNABLES ---
    [Header("Spawning")]
    public GameObject spawnablePrefab;
    
    // --- SPAWN TRANSFORM ---
    private Vector3 _spawnPos;
    private Quaternion _spawnRot;
    private Vector3 _spawnScale = Vector3.one;
    public void SetSpawnPos(Vector3 pos) => _spawnPos = pos;
    public void SetSpawnRot(Vector3 rot) => _spawnRot = Quaternion.Euler(rot);
    public void SetSpawnScale(Vector3 scale) => _spawnScale = scale;
    
    // --- EVENTS ---
    [Header("Spawn Events")]
    public UnityEvent<Transform> onSpawned;
    
    // ------ SPAWNING ------
    
    protected override void InvokeLogic() {
        Transform t = Instantiate(spawnablePrefab, _spawnPos, _spawnRot).transform;
        t.localScale = _spawnScale;
        onSpawned.Invoke(t);
    }
    
    public override void Reset() {}
}
