using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MineAmmo : AbstractAmmo {

    [SerializeField]
    private float explosionLifeTime;
    [SerializeField]
    private float explosionRadius;
    [SerializeField]
    private float explosionKickForce;

    [SerializeField]
    private float prepareTime;

    private bool ready = false;

    public override void Initialize(BulletInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
        StartCoroutine(Prepare());
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!ready)
            return;

        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.TryGetComponent(out ModuleHp moduleHp))
        {
            if (moduleHp.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject
                && NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, bulletInfo))
            {
                new ExplosionManager.Explosion(0, bulletInfo.Damage, 1, 1.5f, 5).Explode(collision.transform.position, bulletInfo.OwnerShip);
                NetworkServer.Destroy(gameObject);
            }
        }
        else
        {
            new ExplosionManager.Explosion(0, bulletInfo.Damage, 1, 1.5f, 5).Explode(collision.transform.position, bulletInfo.OwnerShip);
            NetworkServer.Destroy(gameObject);
        }
    }

    private IEnumerator Prepare()
    {
        yield return new WaitForSeconds(prepareTime);
        ready = true;
    }
}
