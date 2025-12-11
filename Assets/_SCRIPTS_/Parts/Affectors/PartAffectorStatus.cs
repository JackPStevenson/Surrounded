using UnityEngine;

public class PartAffectorStatus : PartAffector
{
    // --- STATUS ---
    [Header("Effect")]
    public bool effectsPermanent = false;
    public DataStatusEffect[] statusEffects;
    public float basePotency = 1;
    private float _potencyMod = 1;


    public void SetPotency(float potency) => _potencyMod = potency;

    // ------ PART FUNCTIONS ------

    protected override void OnCompAffect(Health comp)
    {
        if (comp.TryGetComponent(out StatusHandler handler))
            foreach (DataStatusEffect effect in statusEffects)
                handler.TryAddEffect(effect, !effectsPermanent, basePotency * _potencyMod);
    }

    public override void Reset() { base.Reset(); _potencyMod = 1; }
}