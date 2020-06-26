using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class FreezeEffect : IEffect
{
    private readonly float freezeK;
    private readonly float duration;

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

