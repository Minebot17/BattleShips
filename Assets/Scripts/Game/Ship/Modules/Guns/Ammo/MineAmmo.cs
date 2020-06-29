using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MineAmmo : AbstractAmmo {

    //[SerializeField] private float explosionLifeTime;
    //[SerializeField] private float explosionRadius;
    //[SerializeField] private float explosionKickForce;
    [SerializeField] private float prepareTime;

    private bool ready = false;

    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
        StartCoroutine(Prepare());
    }

    protected override void OnMapTrigger(Collider2D collider) {
        if (ready)
            NetworkServer.Destroy(gameObject);
    }

    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp) {
        if (ready)
            NetworkServer.Destroy(gameObject);
    }

    protected void OnDestroy() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        ExplosionManager.mineAmmoExplosion.Explode(transform.position, damageInfo.OwnerShip);
    }

    private IEnumerator Prepare() {
        yield return new WaitForSeconds(prepareTime);
        ready = true;
    }
}