using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class FireModule : MonoBehaviour, IEffectFabric
{
    [SerializeField] private float damage;
    [SerializeField] private float speed;
    [SerializeField] private float duration;
    [SerializeField] private int chance;

    private DamageInfo damageInfo;

    private void Start()
    {
        damageInfo = new DamageInfo(damage, transform.parent.parent.gameObject.GetComponent<NetworkIdentity>());
    }

    public IEffect Create()
    {
        return new FireEffect(speed, duration, chance, damageInfo);
    }
}
