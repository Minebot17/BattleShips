using UnityEngine;
using UnityEngine.Networking;

public class AbstractModuleNetwork : NetworkBehaviour
{
    protected EffectModule effectModule;
    protected virtual void Start()
    {
        effectModule = GetComponent<EffectModule>();
    }
}