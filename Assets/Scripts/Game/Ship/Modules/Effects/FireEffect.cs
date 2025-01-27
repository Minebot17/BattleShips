﻿
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

internal class FireEffect : IEffect
{
    private readonly float speed;
    private readonly float duration;
    private readonly int chance;

    private readonly DamageInfo damageInfo;
    private readonly System.Random random;

    public FireEffect(float speed, float duration, int chance, DamageInfo damageInfo)
    {
        this.speed = speed;
        this.duration = duration;
        this.chance = chance;
        random = new System.Random();
        this.damageInfo = damageInfo;
    }

    public IEnumerator Start(EffectModule effectModule)
    {
        int r = random.Next(0, 100);
        if(r < chance && effectModule.transform.TryGetComponent(out ModuleHp moduleHp))
        {
            float timer = duration;
            effectModule.effects.Add(this);
            while (timer > 0)
            {
                moduleHp.Damage(damageInfo);
                yield return new WaitForSeconds(speed);
                timer--; 
            }
            effectModule.effects.Remove(this);
        }    
    }
}