using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GrenadeAmmo : AbstractAmmo {

    [SerializeField]
    private float explosionLifeTime;
    [SerializeField]
    private float explosionRadius;
    [SerializeField]
    private float explosionKickForce;

    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
    }

    protected override void OnMapTrigger(Collider2D collider)
    {
        new ExplosionManager.Explosion(0, bulletInfo.Damage, 1, 1.5f, 5).Explode(transform.position, bulletInfo.OwnerShip);
        NetworkServer.Destroy(gameObject);
    }

    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp)
    {
        new ExplosionManager.Explosion(0, bulletInfo.Damage, 1, 1.5f, 5).Explode(transform.position, bulletInfo.OwnerShip);
        NetworkServer.Destroy(gameObject);
    }

}
