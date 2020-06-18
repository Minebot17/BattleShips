
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

class FireEffect : IEffect
{
    readonly float speed;
    readonly float duration;
    readonly int chance;

    private readonly DamageInfo bulletInfo;
    private readonly System.Random random;

    public FireEffect(float speed, float duration, int chance, DamageInfo bulletInfo)
    {
        this.speed = speed;
        this.duration = duration;
        this.chance = chance;
        random = new System.Random();
        this.bulletInfo = bulletInfo;
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
                moduleHp.Damage(bulletInfo);
                yield return new WaitForSeconds(speed);
                timer--; 
            }
            effectModule.effects.Remove(this);
        }    
    }
}