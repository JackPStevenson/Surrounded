using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoSingleton<ManagerScene> {
    public static ManagerScene Instance;
    public event Action<int> OnAboutToLoadScene;

    // ------ START METHODS ------

    protected override void OnAwake() { }
    
    // ------ EVENT METHODS ------

    public void LoadScene(int sceneId) {
        OnAboutToLoadScene?.Invoke(sceneId);
        SceneManager.LoadScene(sceneId);
    }
}