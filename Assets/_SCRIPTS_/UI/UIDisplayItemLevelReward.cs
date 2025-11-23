using UnityEngine;
using UnityEngine.UI;

public class UIDisplayItemLevelReward : UIDisplayItem {
    public Color unlockedColor;
    public Color lockedColor;

    private Slider _slider;
    public Slider Slider {
        get {
            if (!_slider) TryGetComponent(out _slider);
            return _slider;
        }
    }

    public override void SetInfo(DataDisplayable d, bool allowDescLock = false) => SetInfo(d.levelToUnlock, allowDescLock ? 1 : ManagerSaveLoad.GetLevelProgress(), d.icon);
    public void SetInfo(int level, float progress, Sprite icon) { SetName(level + ""); SetSlider(progress); SetColor(progress >= 1); SetIcon(icon); }

    public void SetColor(bool unlocked) => Name.color = unlocked ? lockedColor : unlockedColor;
    public void SetSlider(float value) => Slider?.SetValueWithoutNotify(value);
}