using System;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T> {
    public static T Inst { get; private set; }

    // ------ START METHODS ------

    private void Awake() {
        if (Inst) {
            if (Inst != this) {
                enabled = false;
                gameObject.SetActive(false);
                Destroy(gameObject);
            }
            return;
        }
        Inst = this as T;
        OnAwake();
    }

    protected abstract void OnAwake();
    
    private void OnDestroy() {
        // WARNING: Commented code below causes issues with saving when exiting game abruptly.
        // if (Inst == this) {
        //     Inst = null;
        //     OnDestroyed(true);
        // }
        
        OnDestroyed(false);
    }

    protected abstract void OnDestroyed(bool isDeletedInstance);
}