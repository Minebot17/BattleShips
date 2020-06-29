using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LauncherAmmo : AbstractAmmo {

    [SerializeField] private float redirectRate;

    private Collider2D aim;
    private CapsuleCollider2D detectArea;
    private CircleCollider2D mainCollider;
    private float speed;

    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        detectArea = GetComponent<CapsuleCollider2D>();
        mainCollider = GetComponent<CircleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
        speed = shootVector.magnitude;
        StartCoroutine(Track());
    }


    protected override void OnMapTrigger(Collider2D collider) {
        if (collider.IsTouching(mainCollider))
            NetworkServer.Destroy(gameObject);
    }

    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp) {
        if (collider.IsTouching(mainCollider))
            NetworkServer.Destroy(gameObject);
    }

    private void OnDestroy() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        ExplosionManager.launcherAmmoExplosion.Explode(transform.position.ToVector2());
    }

    private IEnumerator Track() {
        ContactFilter2D filter = new ContactFilter2D {
        useTriggers = true
        };
        while (true) {
            if (aim == null) {
                List<Collider2D> colliders = new List<Collider2D>();
                detectArea.OverlapCollider(filter, colliders);
                colliders.ForEach(c => {
                    if (c.gameObject.TryGetComponent(out ModuleHp moduleHp)
                        && c.gameObject.GetComponentInParent<ShipController>()
                        && NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, damageInfo)
                        && moduleHp.transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject
                        && (aim == null
                            || Vector2.Distance(transform.position, c.transform.position) <
                            Vector2.Distance(transform.position, aim.transform.position)))
                        aim = c;
                }
                );
            }
            else {
                rigidbody2D.AddForce((aim.transform.position - transform.position).normalized * speed / 5,
                ForceMode2D.Impulse
                );
            }

            yield return new WaitForSeconds(redirectRate);
        }
    }
}