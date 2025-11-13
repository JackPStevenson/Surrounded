using UnityEngine;

public interface IUpdateCustom {
    public void UpdateCustom(float deltaTime);
    public void FixedUpdateCustom(float deltaTime, int tick);
}
