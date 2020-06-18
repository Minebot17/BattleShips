using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractAmmo : NetworkBehaviour
{
    [SerializeField]
    protected int lifeSpan;

    protected DamageInfo bulletInfo;

    protected int lifeSpanTimer = 999999;

    protected new Rigidbody2D rigidbody2D;

    virtual public void Initialize(DamageInfo bulletInfo, Vector2 shootVector)
    {
        this.bulletInfo = bulletInfo;
        lifeSpanTimer = lifeSpan;
    }

    public DamageInfo GetInfo()
    {
        return bulletInfo;
    }

    private void FixedUpdate()
    {
        if (!isServer)
            return;

        if (lifeSpanTimer <= 0)
            NetworkServer.Destroy(gameObject);
        else
            lifeSpanTimer--;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collider)
    {
        if (!isServer)
            return;

        if (collider.gameObject.TryGetComponent(out ModuleHp moduleHp))
        {
            if (moduleHp.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject)
                if (NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, bulletInfo))
                {
                    if (collider.gameObject.TryGetComponent(out EffectModule effectModule))
                        effectModule.AddEffects(bulletInfo.effects.Select(e => e.Create()));

                    OnEnemyTrigger(collider, moduleHp);
                }
                else
                    OnFriendTrigger(collider, moduleHp);
        }     
        else
            OnMapTrigger(collider);
    }

    protected virtual void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp) { }
    protected virtual void OnFriendTrigger(Collider2D collider, ModuleHp moduleHp) { }
    protected virtual void OnMapTrigger(Collider2D collider) { }

}
