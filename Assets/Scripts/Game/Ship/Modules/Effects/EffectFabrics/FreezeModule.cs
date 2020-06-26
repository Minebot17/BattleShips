using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FreezeModule : MonoBehaviour, IEffectFabric
{
    [SerializeField] float freezeK = 1;
    [SerializeField] float duration;
    
    public IEffect Create()
    {
        return new FreezeEffect(freezeK, duration);
    }
}
