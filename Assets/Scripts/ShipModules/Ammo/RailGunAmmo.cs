using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class RailgunAmmo : AbstractAmmo {

    [SerializeField]
    private int pierceAmount;

    public override void Initialize(DamageInfo playerDamageSource, Vector2 shootVector) {
        base.Initialize(playerDamageSource, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
    }

    protected override void OnTriggerEnter2D(Collider2D collider)
    {
        base.OnTriggerEnter2D(collider);

        if (pierceAmount == 0)
            NetworkServer.Destroy(gameObject);
    }

    protected override void OnMapTrigger(Collider2D collider)
    {
        pierceAmount--;
    }
    protected override void OnEnemyTrigger(Collider2D collider, ModuleHp moduleHp)
    {
        moduleHp.Damage(GetInfo());
        pierceAmount--;
    }
}
