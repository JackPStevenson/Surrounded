using System;
using UnityEngine.SceneManagement;

public class ManagerScene : MonoSingleton<ManagerScene>
{
    public event Action<int> OnAboutToLoadScene;

    // ------ START METHODS ------

    protected override void OnAwake() { }
    protected override void OnDestroyed(bool isDeletedInstance) { }

    // ------ EVENT METHODS ------

    public void LoadScene(int sceneId)
    {
        OnAboutToLoadScene?.Invoke(sceneId);
        SceneManager.LoadScene(sceneId);
        if (sceneId == 0)
        {
            ManagerAudio.SetCurrentTrack(2);
            ManagerAudio.UpdateMusicVolume();
        }
    }
}