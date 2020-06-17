using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class FreezeModule : MonoBehaviour, IModuleEffect
{
    [SerializeField] private float freezeK = 1;
    [SerializeField] private float duration;
    
    public FreezeEffect Create()
    {
        return new FreezeEffect(freezeK, duration);
    }
}
