using System;
using UnityEngine;
using UnityEngine.Events;

public class PartSpawnerMoverPath : Part {
    // --- SPAWNING ---
    [Header("Spawning")]
    public GameObject pathMoverPrefab;
    private Vector3[] _path = Array.Empty<Vector3>();
    public void SetPath(Vector3[] path) => _path = path;
    
    // --- EVENTS ---
    [Header("Spawn Events")]
    public UnityEvent<PartMoverPath> onSpawned;
    
    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() {
        if(_path.Length < 2) return;
        Quaternion spawnRot = Quaternion.LookRotation((_path[1] - _path[0]).normalized);
        Instantiate(pathMoverPrefab, _path[0], spawnRot).TryGetComponent(out PartMoverPath pathMover);
        pathMover.SetPath(_path);
        onSpawned.Invoke(pathMover);
    }
    public override void Reset() { _path = Array.Empty<Vector3>(); }
}
