using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class StandartAmmo : AbstractAmmo {

    [SerializeField]
    private int numberOfBounces;

    private new Rigidbody2D rigidbody2D;

    public override void Initialize(BulletInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
    }

    public override void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        if (collision.gameObject.TryGetComponent(out ModuleHp moduleHp))
        {
            if (moduleHp.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject)
            {
                moduleHp.Damage(GetInfo());
                numberOfBounces--;
            }
        }
        else numberOfBounces--;

        if (numberOfBounces == 0)
            NetworkServer.Destroy(gameObject);
    }
}
