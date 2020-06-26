
using UnityEngine;

public class AbstractModule : MonoBehaviour
{
    protected EffectModule effectModule;
    protected virtual void Start()
    {
        effectModule = GetComponent<EffectModule>();
    }
}

