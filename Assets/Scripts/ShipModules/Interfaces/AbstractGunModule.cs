using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractGunModule : AbstractModule {

    [SerializeField] int coolDown = 0;
    [SerializeField] float recoilForce = 0;
    [SerializeField] protected int damage;

    protected BulletInfo bulletInfo;
    int timerCoolDown;
    Rigidbody2D rigidbody;

    protected override void Start() {
        base.Start();
        bulletInfo = new BulletInfo(damage, transform.parent.parent.gameObject.GetComponent<NetworkIdentity>()) {
        effects = GetComponents<IModuleEffect>().ToList()
        };
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
    }

    public void TryShoot(Vector2 vec) {
        if (!NetworkManagerCustom.singleton.IsServer || timerCoolDown > 0)
            return;

        if (timerCoolDown <= 0) {
            Shoot(vec);
            rigidbody.AddForce(-vec * recoilForce, ForceMode2D.Impulse);
            timerCoolDown = (int) (coolDown * effectModule.freezeK);
        }
    }

    public void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerCoolDown > 0)
            timerCoolDown--;
    }

    protected abstract void Shoot(Vector2 vec);
}