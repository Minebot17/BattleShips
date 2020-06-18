using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

class SimpleSpikes : NetworkBehaviour, IMeleeModule
{
    [SerializeField] protected int damage;

    protected DamageInfo bulletInfo;
    private new Rigidbody2D rigidbody;

    protected void Start()
    {
        bulletInfo = new DamageInfo(damage, transform.parent.parent.gameObject.GetComponent<NetworkIdentity>())
        {
            effects = GetComponents<IEffectFabric>().ToList()
        };
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        if (collider.gameObject.TryGetComponent(out ModuleHp moduleHp))
            if (moduleHp.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject)
                if (NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, bulletInfo))
                {
                    if (collider.gameObject.TryGetComponent(out EffectModule effectModule))
                        effectModule.AddEffects(bulletInfo.effects.Select(e => e.Create()));
                    bulletInfo.Damage *= rigidbody.velocity.magnitude;
                    moduleHp.Damage(bulletInfo);
                }
    }
}

