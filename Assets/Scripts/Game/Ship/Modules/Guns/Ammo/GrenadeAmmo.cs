using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class GrenadeAmmo : AbstractAmmo {

    //[SerializeField] private float explosionLifeTime;
    //[SerializeField] private float explosionRadius;
    //[SerializeField] private float explosionKickForce;

    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
    }

    protected override void OnMapTrigger(Collider2D collider) {
        NetworkServer.Destroy(gameObject);
    }

    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp) {
        NetworkServer.Destroy(gameObject);
    }

    protected void OnDestroy() {
        ExplosionManager.grenadeAmmoExplosion.Explode(transform.position, damageInfo.OwnerShip);
    }
}
