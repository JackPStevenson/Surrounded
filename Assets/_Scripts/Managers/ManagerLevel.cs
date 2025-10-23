using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagerLevel : MonoBehaviour {
    public static ManagerLevel Instance;
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