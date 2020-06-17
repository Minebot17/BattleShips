using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractAmmo : NetworkBehaviour
{
    [SerializeField]
    protected int lifeSpan;

    protected BulletInfo bulletInfo;

    protected int lifeSpanTimer = 999999;

    protected new Rigidbody2D rigidbody2D;

    virtual public void Initialize(BulletInfo bulletInfo, Vector2 shootVector)
    {
        this.bulletInfo = bulletInfo;
        lifeSpanTimer = lifeSpan;
    }

    public BulletInfo GetInfo()
    {
        return bulletInfo;
    }

    public virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer)
            return;

        if (collision.gameObject.TryGetComponent(out EffectModule effectModule) 
            && effectModule.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject)
        {
            effectModule.AddEffects(bulletInfo.effects.Select(e => e.Create()));
        }
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
}
