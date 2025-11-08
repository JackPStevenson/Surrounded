using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PartMoverPath : Part {
    [Header("Movement")]
    public float travelSpeed = 5;
    public bool tryStartOnSpawn = true;
    
    private Vector3[] _path;
    public void SetPath(Vector3[] path) { _path = path; for (int i = 0; i < path.Length - 1; i++) _pathLength += GetSegmentLength(i); }
    private float _pathLength;
    private float _lengthTraversed;
    
    // ------ START FUNCTIONS ------
    
    protected override void OnStart() { if(tryStartOnSpawn) TryInvoke(); }
    
    // ------ UPDATE FUNCTIONS ------
    
    void FixedUpdate() {
        if (!Activated) return;
        
        if (_path is not {Length: > 0} || _lengthTraversed >= _pathLength) {
            Destroy();
            return;
        }

        _lengthTraversed += Time.fixedDeltaTime * travelSpeed;
        
        transform.position = GetPathPos(_lengthTraversed);
        Vector3 aimPos = GetPathPos(_lengthTraversed + 0.5f);
        if(Vector3.Distance(transform.position, aimPos) > 0.01f)
            transform.forward = aimPos - GetPathPos(_lengthTraversed);
    }
    
    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() => Activated = true;
    public override void Reset() => _lengthTraversed = 0;
    void Destroy() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }

    // ------ HELPER FUNCTIONS ------
    
    float GetSegmentLength(int segment) => Vector3.Distance(_path[segment], _path[segment + 1]);
    Vector3 GetPathPos(float distFromStart) {
        float dist = distFromStart;
        for (int i = 0; i < _path.Length - 1; i++) {
            float segment = GetSegmentLength(i);
            if (segment >= dist) return Vector3.Lerp(_path[i], _path[i + 1], dist / segment);
            dist -= segment;
        }
        return _path[^1];
    }
}