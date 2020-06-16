using UnityEngine;
using UnityEngine.Networking;

public class StandartAmmo : AbstractAmmo {

    [SerializeField]
    private int numberOfBounces;

    private new Rigidbody2D rigidbody2D;

    public override void Initialize(GameObject owner, Vector2 shootVector) {
        base.Initialize(owner, shootVector);
        rigidbody2D = GetComponent<Rigidbody2D>();
        rigidbody2D.AddForce(shootVector, ForceMode2D.Impulse);
    }

    public override void OnCollide(ModuleHp hp) {
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isServer)
            return;

        ModuleHp moduleHp;
        if (collision.gameObject.TryGetComponent(out moduleHp))
        {
            if (moduleHp.transform.parent.parent.gameObject != owner)
            {
                moduleHp.Damage(GetDamageSource());
            }
            else numberOfBounces--;

        }
        else numberOfBounces--;

        if (numberOfBounces == 0)
            NetworkServer.Destroy(gameObject);
    }
}
