using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractGunModule : AbstractModule, IGunModule {

    [SerializeField] protected float coolDown = 0;
    [SerializeField] float recoilForce = 0;
    [SerializeField] protected int damage;

    protected DamageInfo bulletInfo;
    protected float timerCoolDown;
    private new Rigidbody2D rigidbody;

    protected override void Start() {
        base.Start();
        bulletInfo = new DamageInfo(damage, transform.parent.parent.gameObject.GetComponent<NetworkIdentity>()) {
            effects = GetComponents<IEffectFabric>().ToList()
        };
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
    }

    public void TryShoot(Vector2 vec) {
        if (!NetworkManagerCustom.singleton.IsServer || timerCoolDown > 0)
            return;

        timerCoolDown = coolDown * effectModule.freezeK;
        Shoot(vec);
        rigidbody.AddForce(-vec * recoilForce, ForceMode2D.Impulse);
    }

    public virtual void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerCoolDown > 0)
            timerCoolDown -= Time.fixedDeltaTime; 
    }

    protected abstract void Shoot(Vector2 vec);
}