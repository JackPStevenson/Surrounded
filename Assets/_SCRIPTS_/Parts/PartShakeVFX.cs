public class PartShakeVFX : Part
{
    public void PlayShakeVFX()
    {
        ParticleManager.Inst.PlayShakeEffect();
    }

    public void StopShakeVFX()
    {
        ParticleManager.Inst.StopShakeEffect();
    }

    public override void Reset()
    {

    }

    protected override void InvokeLogic()
    {
        StopShakeVFX();
    }
}
