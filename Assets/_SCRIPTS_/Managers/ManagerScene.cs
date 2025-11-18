using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoBehaviour {
    public static ManagerScene Instance;
    public event Action<int> OnAboutToLoadScene;

    // ------ START METHODS ------

    void Awake() {
        Instance = this;
    }

    void Start() {

    }

    // ------ UPDATE METHODS ------

    void Update() {

    }

    // ------ EVENT METHODS ------

    public void LoadScene(int sceneId) {
        OnAboutToLoadScene?.Invoke(sceneId);
        SceneManager.LoadScene(sceneId);
    }
}