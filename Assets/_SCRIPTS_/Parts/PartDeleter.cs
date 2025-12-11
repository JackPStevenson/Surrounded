using UnityEngine;

public class PartDeleter : Part {
    protected override void InvokeLogic() {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
    public override void Reset() { }
}
