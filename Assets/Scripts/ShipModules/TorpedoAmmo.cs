using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TorpedoAmmo : AbstractAmmo {

    [SerializeField] private float skidForce;

    private Vector2 shootVector;

    public override void Initialize(BulletInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        this.shootVector = shootVector;
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
        StartCoroutine(TorpedoMove());
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.TryGetComponent(out ModuleHp moduleHp))
        {
            if (moduleHp.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject
              && NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, bulletInfo))
            {       
                moduleHp.Damage(GetInfo());
                NetworkServer.Destroy(gameObject);
            }
        }
        else NetworkServer.Destroy(gameObject);
    }

    private IEnumerator TorpedoMove()
    {
        float startTime = 0;
        rigidbody2D.AddForce(Quaternion.Euler(0, 0, 90) * shootVector.normalized * skidForce * 7.95f, ForceMode2D.Force);
        while (lifeSpanTimer >= 0)
        {
            startTime += Time.fixedDeltaTime;
            rigidbody2D.AddForce(Quaternion.Euler(0, 0, 90) * shootVector.normalized * skidForce * -(float)Math.Sin(startTime * Math.PI * 2), ForceMode2D.Force);
            yield return new WaitForFixedUpdate();
        }
    }
}
