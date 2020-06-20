using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LauncherAmmo : AbstractAmmo {

    [SerializeField] private float redirectRate;

    private Collider2D aim;
    private CapsuleCollider2D detectArea;
    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        detectArea = GetComponent<CapsuleCollider2D>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
        StartCoroutine(Track());
    }
    

    protected override void OnMapTrigger(Collider2D collider)
    {
        NetworkServer.Destroy(gameObject);
    }
    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp)
    {
        moduleHp.Damage(GetInfo());
        NetworkServer.Destroy(gameObject);
    }

    private IEnumerator Track()
    {
        while (true)
        {
            if(aim == null)
            {
                Collider2D[] colliders = { };
                detectArea.OverlapCollider(new ContactFilter2D(), colliders);
                aim = colliders.Length > 0 ? colliders.Aggregate(
                    (nearest, c) =>
                    c.gameObject.TryGetComponent(out ModuleHp moduleHp)
                    && NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, damageInfo)
                    && moduleHp.transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject
                    && (nearest == null 
                    || Vector2.Distance(transform.position, c.transform.position) < Vector2.Distance(transform.position, nearest.transform.position)) 
                    ? c 
                    : nearest) : null;
            }
            else
            {
                rigidbody2D.AddForce(aim.transform.position, ForceMode2D.Impulse);
            }
            yield return new WaitForSeconds(redirectRate);
        }
    }
}
