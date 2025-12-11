using UnityEngine;

public class PartLogicCounter : Part {
    // --- COUNTING ---
    [Header("Counting")]
    public int maxCount = 3;
    public int minCountForActivation = -1; // 0 or lower makes counter activate when current count reaches max.
    public bool loopCounter = true;
    private int _currentCount;

    // ------ PART FUNCTIONS ------
    
    public void SetMaxCount(float count) => maxCount = Mathf.RoundToInt(count);
    
    protected override void InvokeLogic() => ModCount(1);
    public void ModCount(int modifier) {
        _currentCount = Mathf.Clamp(_currentCount + modifier, 0, maxCount);
        Activated = _currentCount >= maxCount;
        print(1);
        if (Activated && loopCounter) _currentCount = 0;
    }
    public override void Reset() {
        Activated = false;
        ModCount(-maxCount);
    }
}