using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractAmmo : NetworkBehaviour
{
    [SerializeField]
    protected int lifeSpan;

    protected DamageInfo damageInfo;

    protected int lifeSpanTimer = 999999;

    protected new Rigidbody2D rigidbody2D;

    public virtual void Initialize(DamageInfo damageInfo, Vector2 shootVector)
    {
        this.damageInfo = damageInfo;
        lifeSpanTimer = lifeSpan;
    }

    public DamageInfo GetInfo()
    {
        return damageInfo;
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
            if (!damageInfo.OwnerShip || moduleHp.transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject)
                if (NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, damageInfo))
                {
                    if (collider.gameObject.TryGetComponent(out EffectModule effectModule))
                        effectModule.AddEffects(damageInfo.effects.Select(e => e.Create()));

                    OnEnemyTrigger(collider, moduleHp);
                }
                else
                    OnFriendTrigger(collider, moduleHp);
        }     
        else if (!collider.gameObject.CompareTag("TransparentForBullets"))
            OnMapTrigger(collider);
    }

    protected virtual void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp) { }
    protected virtual void OnFriendTrigger(Collider2D collider, ModuleHp moduleHp) { }
    protected virtual void OnMapTrigger(Collider2D collider) { }

}
