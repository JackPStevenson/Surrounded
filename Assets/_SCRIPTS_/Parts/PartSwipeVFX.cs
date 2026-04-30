using UnityEngine;

public class PartSwipeVFX : Part
{
    public void PlaySwipeVFX(Vector3[] pos)
    {
        ParticleManager.Inst.PlaySwipeEffect(pos);
    }

    public void StopSwipeVFX()
    {
        ParticleManager.Inst.StopSwipeEffect();
    }

    public override void Reset()
    {

    }

    protected override void InvokeLogic()
    {

    }
}
