using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartLogicCounter : Part {
    // --- COUNTING ---
    [Header("Counting")]
    public int maxCount = 3;
    public int minCountForActivation = -1; // 0 or lower makes counter activate when current count reaches max.
    public bool loopCounter = true;
    private int _currentCount;

    // ------ PART FUNCTIONS ------
    
    protected override void InvokeLogic() => ModCount(1);
    public void ModCount(int modifier) {
        _currentCount = Mathf.Clamp(_currentCount + modifier, 0, maxCount);
        if (loopCounter) _currentCount %= (maxCount + 1);
        Activated = _currentCount >= (minCountForActivation > 0 ? Mathf.Min(maxCount, minCountForActivation) : maxCount);
    }
    public override void Reset() => ModCount(-maxCount);
}