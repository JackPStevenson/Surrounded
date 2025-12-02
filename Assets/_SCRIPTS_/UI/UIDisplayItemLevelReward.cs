using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItemLevelReward : UIDisplayItem {
    public Color unlockedColor;
    public Color lockedColor;

    private Slider _slider;
    public Slider Slider => ChildAutoFetch<Slider>(_slider, "Slider");

    private Transform _nextReward;
    protected Transform NextReward => ChildAutoFetch(_nextReward, "Next Reward");
    //{ get { if (!_nextReward) _nextReward = transform.Find("Next Reward")?.gameObject; return _nextReward; } }

    // ------ SETUP METHODS ------
    
    public override void SetInfo(DataDisplayable d, bool allowDescLock = false) => SetInfo(d.levelToUnlock, allowDescLock ? 1 : ManagerSaveLoad.GetLevelProgress(), d.icon);
    public void SetInfo(int rewardLevel, Sprite icon) => SetInfo(rewardLevel, Mathf.Clamp01((ManagerSaveLoad.GetLevel() - (rewardLevel - 1)) + ManagerSaveLoad.GetLevelProgress()), icon);
    public void SetInfo(int rewardLevel, float progress, Sprite icon) { SetName(rewardLevel + ""); SetSlider(progress); SetColor(progress >= 1); SetIcon(icon); ToggleNextReward(rewardLevel == ManagerSaveLoad.GetLevel() + 1); }

    // ------ SINGLE SETUP METHODS ------

    public void SetColor(bool unlocked) { if (Name) Name.color = unlocked ? unlockedColor : lockedColor; }
    public void SetSlider(float value) => Slider?.SetValueWithoutNotify(value);
    public void ToggleNextReward(bool active) => NextReward?.gameObject.SetActive(active);
}