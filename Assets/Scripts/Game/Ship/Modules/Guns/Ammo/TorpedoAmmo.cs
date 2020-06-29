using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TorpedoAmmo : AbstractAmmo {

    [SerializeField] private float skidForce;

    private Vector2 shootVector;

    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        this.shootVector = shootVector;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
        StartCoroutine(TorpedoMove());
    }

    protected void OnCollisionEnter2D(Collision2D other) {
        NetworkServer.Destroy(gameObject);
    }
    
    protected override void OnMapTrigger(Collider2D collider) {
        NetworkServer.Destroy(gameObject);
    }

    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp) {
        NetworkServer.Destroy(gameObject);
    }

    protected void OnDestroy() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        ExplosionManager.torpedoAmmoExplosion.Explode(transform.position, GetInfo().OwnerShip);
    }

    private IEnumerator TorpedoMove() {
        float startTime = 0;
        rigidbody2D.AddForce(Quaternion.Euler(0, 0, 90) * shootVector.normalized * (skidForce * 7.95f), ForceMode2D.Force);
        while (lifeSpanTimer >= 0) {
            startTime += Time.fixedDeltaTime;
            rigidbody2D.AddForce(Quaternion.Euler(0, 0, 90) * shootVector.normalized * (skidForce * -(float) Math.Sin(startTime * Math.PI * 2)), ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
    }
}