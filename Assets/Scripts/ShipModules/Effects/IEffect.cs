using System.Collections;

public interface IEffect
{
    IEnumerator Start(EffectModule effectModule);
}

