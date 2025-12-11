using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartSpawnerObj : Part {
    // --- SPAWNING ---
    [Header("Spawning")]
    public GameObject spawnablePrefab;
    private Vector3 _spawnPos;
    private Quaternion _spawnRot;
    private Vector3 _spawnScale = Vector3.one;

    public float spawnableExtraValue = 0;
    
    public void SetSpawnableExtraValue(float value) => spawnableExtraValue = value;
    public void SetSpawnPos(Vector3 pos) => _spawnPos = pos;
    public void SetSpawnRot(Vector3 rot) => _spawnRot = Quaternion.Euler(rot);
    public void SetSpawnScale(Vector3 scale) => _spawnScale = scale;
    
    // --- EVENTS ---
    [Header("Spawn Events")]
    public UnityEvent<Transform> EventSpawned;
    
    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() {
        Transform t = Instantiate(spawnablePrefab, _spawnPos, _spawnRot).transform;
        t.localScale = _spawnScale;
        
        if (t.TryGetComponent(out PartLogicValue part) && part.CompareName("Extra"))
            part.SetValue(spawnableExtraValue);
        
        EventSpawned.Invoke(t);
    }
    
    public override void Reset() {
        spawnableExtraValue = 0;
    }
}
