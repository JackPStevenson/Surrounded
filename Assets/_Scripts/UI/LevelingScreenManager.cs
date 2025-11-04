using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelingScreenManager : MonoBehaviour
{
    [SerializeField] private Transform levelProgressObject;
    [SerializeField] private TMP_Text levelText;

    public static LevelingScreenManager instance;
    //public int level = 1;
    //public int experience = 1000;
    //public int xpReq = 2000;

    public bool a = false;
    public bool b = false;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

    }

    private void Update()
    {
        if (a)
        {
            a = false;
            UpdateLevelDisplay();
        }
        if (b)
        {
            b = false;
            PlayerLevel.instance.AddExperience((uint)10000);
        }
    }
    public void UpdateLevelDisplay()
    {
        // get player level
        int level = (int)PlayerLevel.instance.CurrentLevel;
        // get player experience
        int experience = (int)PlayerLevel.instance.Experience;
        // get xp requirement for next level
        int xpReq = (int)PlayerLevel.instance.GetCurrentXPRequirement();
        levelText.SetText("Current Level\n" + level.ToString());
        // loop through all the sliders
        int numChildren = levelProgressObject.childCount;
        for (int i = 0; i <= level && i < numChildren; i++)
        {
            //Debug.Log("index: " + i + "\nchildren: " + numChildren);
            if (i < numChildren)
            {
                Slider slider = levelProgressObject.GetChild(i).GetComponentInChildren<Slider>();

                if (i == level)
                {
                    // get the percentage
                    float percentComplete = (experience * 1f) / (xpReq * 1f);
                    Debug.Log(percentComplete);
                    if (percentComplete > 1)
                        percentComplete = 1;
                    // change slider to partial
                    slider.value = percentComplete;
                }
                else
                {
                    // set slider to full
                    slider.value = 1;
                }
            }
        }
    }
}
