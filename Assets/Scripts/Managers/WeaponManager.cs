using System;
using UnityEngine;
using UnityEngine.Serialization;

public class WeaponManager : MonoBehaviour {
    private InputManager _input;
    
    [Header("Physics")]
    public LayerMask hitMask;
    public float attackTestHeight = 1;

    [Header("Weapons")]
    public WeaponDataEntry currentWeapon;
    public Transform debugTracker;
    
    private Camera _camera;
    
    private Collider[] _hitObjects;

    void Start() {
        _camera = Camera.main;
        
        _input = InputManager.Instance;
        _input.TouchPressDelegate += TapAttack;
        _input.TouchPositionDelegate += SwipeAttack;
        
        _hitObjects = new Collider[250];
    }

    void Update() {

    }


    void TapAttack(Vector2 input) {
        // Only do tap attack if current weapon attacks that way.
        if (currentWeapon.attackType is not AttackType.Tap) return;
        
        // Convert screen-space touch location to viewport position above floor.
        Vector3 viewPos = _camera.ScreenToViewportPoint(input);
        viewPos.z = _camera.transform.position.y - attackTestHeight;
        
        // Convert viewport position above ground to world position.
        Vector3 worldPos = _camera.ViewportToWorldPoint(viewPos);
        
        // Only continue if something was hit.
        int hitObjCount = Physics.OverlapSphereNonAlloc(worldPos, currentWeapon.radius, _hitObjects, hitMask);
        print(hitObjCount);
        
        
        debugTracker.position = worldPos;
        if(hitObjCount <= 0) return;

        ZombieBase z;
        for (int i = 0 ; i < hitObjCount; i++) {
            if (!_hitObjects[i].TryGetComponent(out z)) continue;
            z.DealDamage(currentWeapon.damage);
        }
        
        
        Array.Clear(_hitObjects, 0 , hitObjCount);
    }
    
    void SwipeAttack(Vector2 input) {
        // Only do tap attack if current weapon attacks that way.
        if (currentWeapon.attackType is not AttackType.Tap) return;
        
        // Convert screen-space touch location to viewport position above floor.
        Vector3 viewPos = _camera.ScreenToViewportPoint(input);
        viewPos.z = _camera.transform.position.y - attackTestHeight;
        
        // Convert viewport position above ground to world position.
        Vector3 worldPos = _camera.ViewportToWorldPoint(viewPos);
    }

    public void SetCurrentWeapon(WeaponDataEntry newWeapon) {
        currentWeapon = newWeapon;
        
    }
}