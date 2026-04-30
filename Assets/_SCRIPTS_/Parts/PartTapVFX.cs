using UnityEngine;

public class PartTapVFX : Part
{
    public void PlayTapVFX(Vector3 pos)
    {
        ParticleManager.Inst.PlayTapEffect(pos);
    }

    public void StopTapVFX()
    {
        ParticleManager.Inst.StopTapEffect();
    }

    public override void Reset()
    {

    }

    protected override void InvokeLogic()
    {

    }
}
