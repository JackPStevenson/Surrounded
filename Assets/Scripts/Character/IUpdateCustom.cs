using UnityEngine;

public interface IUpdateCustom {
    public void UpdateCustom(float deltaTime);
    public void FixedUpdateCustom(int tick, float deltaTime);
}
