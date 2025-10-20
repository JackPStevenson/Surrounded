using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class PartCounter : PartBase {
    // --- COUNTING ---
    [Header("Counting")]
    public int maxCount = 3;
    public int minCountForActivation = -1; // 0 or lower makes counter activate when current count reaches max.
    public bool loopCounter = true;
    private int _currentCount;

    // ------ COUNT INCREMENTING ------
    
    /// Increments counter by 1 when invoked.
    protected override void InvokeLogic() => ModCount(1);
    
    /// Modifies count by given amount.
    public void ModCount(int modifier) {
        _currentCount = (_currentCount + modifier) % (maxCount + 1);
        
        // If minimum count for activation is 0 or lower, mark Activated when current count reaches max count.
        if (minCountForActivation <= 0) Activated = _currentCount == maxCount;
        // Otherwise, mark Activated when current count reaches or exceeds minimum activation count.
        else Activated = _currentCount >= Mathf.Min(maxCount, minCountForActivation);
    }

    /// Resets count to 0.
    public override void Reset() => ModCount(-_currentCount);
}