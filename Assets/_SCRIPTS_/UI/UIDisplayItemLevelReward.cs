using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItemLevelReward : UIDisplayItem {
    public Color unlockedColor;
    public Color lockedColor;

    private Slider _slider;
    public Slider Slider => ChildAutoFetch<Slider>(_slider, "Slider");

    private Transform _nextReward;
    protected GameObject NextReward => ChildAutoFetch(_nextReward, "Next Reward").gameObject;

    // ------ SETUP METHODS ------
    
    public override void SetInfo(DataDisplayable d, bool allowLock = false) => SetInfo(d, d.levelToUnlock, ManagerSaveLoad.GetLevelProgress());
    public void SetInfo(DataDisplayable d, int level) { SetInfo(d, level, ManagerSaveLoad.GetLevelProgress()); }
    public void SetInfo(DataDisplayable d, int level, float progress) { base.SetInfo(d); SetLevel(level, progress); }
    
    public void SetLevel(int level, float progress) { float realProgress = progress + ((ManagerSaveLoad.GetLevel(1)) - level); SetDesc(level.ToString()); SetSlider(realProgress); SetDescColor(realProgress); ToggleNext(realProgress); }
    
    // ------ SINGLE SETUP METHODS ------

    public void SetDescColor(float progress) => SetDescColor(progress >= 1 ? unlockedColor : lockedColor);
    public void SetSlider(float value) => Slider?.SetValueWithoutNotify(Mathf.Clamp01(value));
    public void ToggleNext(float progress) => NextReward?.SetActive(progress is >= 0 and < 1);
}