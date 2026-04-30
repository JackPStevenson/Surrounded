using TMPro;
using UnityEngine;

public class SuperSillyLevelDisplay : MonoBehaviour {
    public TMP_Text levelText;
    public TMP_Text progressToNextText;

    void FixedUpdate() {
        levelText.text = "Level: " + ManagerSaveLoad.GetLevel();
        progressToNextText.text = "Progress to Level " + (ManagerSaveLoad.GetLevel() + 1) + ": " + Mathf.CeilToInt(((float) ManagerSaveLoad.GetExperience() / SaveLoadPlayer.ExperiencePerLevel) * 100) + "%";
    }
}