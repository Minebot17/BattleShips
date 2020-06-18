using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

class Flamethrower : AbstractModule, IGunModule
{
    [SerializeField] float recoilForce = 0;
    [SerializeField] protected int damage;
    [SerializeField] protected float damageSpeed;

    protected DamageInfo bulletInfo;
    private new Rigidbody2D rigidbody;

    private new ParticleSystem particleSystem;
    private float timerFiring = 0;
    private Collider2D fireCollider;
    protected override void Start()
    {
        base.Start();
        bulletInfo = new DamageInfo(damage, transform.parent.parent.gameObject.GetComponent<NetworkIdentity>())
        {
            effects = GetComponents<IEffectFabric>().ToList()
        };
        rigidbody = GetComponentInParent<Rigidbody2D>();
        fireCollider = GetComponent<CapsuleCollider2D>();
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Stop();
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        StartCoroutine(Fire());
    }
   
    public void TryShoot(Vector2 vec)
    {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        timerFiring = damageSpeed;
        rigidbody.AddForce(-vec * recoilForce, ForceMode2D.Impulse);
    }

    public virtual void FixedUpdate()
    {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerFiring > 0)
        {
            particleSystem.Play();
        }
        else particleSystem.Stop();

        timerFiring -= Time.fixedDeltaTime;
    }

    private IEnumerator Fire()
    {     
        while (true)
        {
            if (timerFiring > 0)
            {
                Collider2D[] colliders = { };
                fireCollider.OverlapCollider(new ContactFilter2D(), colliders);
                foreach(Collider2D collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent(out ModuleHp moduleHp))
                    {
                        if (moduleHp.transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject)
                            if (NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, bulletInfo))
                            {
                                if (collider.gameObject.TryGetComponent(out EffectModule effectModule))
                                    effectModule.AddEffects(bulletInfo.effects.Select(e => e.Create()));

                                moduleHp.Damage(bulletInfo);
                            }
                    }
                }
            }
            yield return new WaitForSeconds(damageSpeed);
        }    
    }
}

