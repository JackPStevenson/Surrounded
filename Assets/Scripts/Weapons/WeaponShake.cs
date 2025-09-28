using UnityEngine;

public class WeaponShake : WeaponBase {

    protected override void ShakeAction(Vector3 strength) {
        print(strength.magnitude);
    } 
    
    
}
