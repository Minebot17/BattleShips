using System.Collections.Generic;
using UnityEngine;

public class EffectModule : MonoBehaviour {
    public readonly List<IEffect> effects = new List<IEffect>();

    public float freezeK = 1;

    public void AddEffect(IEffect effect) {
        StartCoroutine(effect.Start(this));
    }

    public void RemoveEffect(IEffect effect) {
        effects.Remove(effect);
    }

    public void AddEffects(IEnumerable<IEffect> effects) {
        foreach (IEffect effect in effects)
            StartCoroutine(effect.Start(this));
    }
}