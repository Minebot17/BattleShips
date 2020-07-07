using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

internal class SimpleSpikes : MonoBehaviour, IMeleeModule
{
    [SerializeField] protected int damage;

    protected DamageInfo damageInfo;
    private new Rigidbody2D rigidbody;

    protected void Start()
    {
        damageInfo = new DamageInfo(damage, transform.parent.parent.parent.gameObject.GetComponent<NetworkIdentity>())
        {
            effects = GetComponents<IEffectFabric>().ToList()
        };
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
    }

    public void OnTriggerEnter2D(Collider2D collider)
    {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (collider.gameObject.transform.childCount == 1 && collider.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out ModuleHp moduleHp))
            if (moduleHp.transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject)
                if (NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, damageInfo))
                {
                    if (collider.gameObject.transform.GetChild(0).gameObject.TryGetComponent(out EffectModule effectModule))
                        effectModule.AddEffects(damageInfo.effects.Select(e => e.Create()));
                    damageInfo.Damage *= (rigidbody.velocity + collider.gameObject.transform.parent.gameObject.GetComponent<Rigidbody2D>().velocity).magnitude;
                    rigidbody.velocity *= 0.75f;
                    rigidbody.MarkServerChange();
                    moduleHp.Damage(damageInfo);
                }
    }
}

