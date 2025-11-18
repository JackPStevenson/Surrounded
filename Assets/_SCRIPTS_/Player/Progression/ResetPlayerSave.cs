using UnityEngine;

public class ResetPlayerSave : MonoBehaviour {
    public void Reset() => ManagerSaveLoad.ClearPlayerSave();
}
