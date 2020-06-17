using System.Collections;
using UnityEngine;

public class FreezeEffect : IEffect
{
    readonly float freezeK;
    readonly float duration;

    public FreezeEffect(float freezeK, float duration)
    {
        this.freezeK = freezeK;
        this.duration = duration;
    }

    public IEnumerator Start(EffectModule effectModule)
    {
        effectModule.effects.Add(this);
        effectModule.freezeK *= freezeK;
        yield return new WaitForSeconds(duration);
        effectModule.freezeK /= freezeK;
        effectModule.effects.Remove(this);
    }
}

