using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    // ------ START METHODS ------
    
    void Awake() {
        Instance = this;
    }
    
    void Start()
    {
        
    }

    // ------ UPDATE METHODS ------
    
    void Update()
    {
        
    }
    
    // ------ EVENT METHODS ------

    public void LoadScene(int sceneId) {
        SceneManager.LoadScene(sceneId);
    }
}
