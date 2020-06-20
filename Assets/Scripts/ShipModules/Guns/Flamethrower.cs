using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

class Flamethrower : AbstractModule, IGunModule
{
    [SerializeField] float recoilForce = 0;
    [SerializeField] protected int damage;
    [SerializeField] protected float damageSpeed;

    protected DamageInfo damageInfo;
    private new Rigidbody2D rigidbody;

    private new ParticleSystem particleSystem;
    private float timerFiring = 0;
    private Collider2D fireCollider;
    private bool isFiring;
    protected override void Start()
    {
        base.Start();
        damageInfo = new DamageInfo(damage, transform.parent.parent.gameObject.GetComponent<NetworkIdentity>())
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
        timerFiring = damageSpeed;

        rigidbody.AddForce(-vec * recoilForce, ForceMode2D.Impulse);
    }

    public virtual void FixedUpdate()
    {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (timerFiring > 0 && !isFiring)
        {
            isFiring = true;
            particleSystem.Play();
            new FlamethrowerClientMessage(damageInfo.OwnerShip, transform.parent.GetSiblingIndex(), true).SendToAllClient();        
        }
        else if(timerFiring < 0 && isFiring)
        {
            particleSystem.Stop();
            new FlamethrowerClientMessage(damageInfo.OwnerShip, transform.parent.GetSiblingIndex(), false).SendToAllClient();
            isFiring = false;
        }
        timerFiring -= Time.fixedDeltaTime;
    }

    private IEnumerator Fire()
    {
        ContactFilter2D filter = new ContactFilter2D()
        {
            useTriggers = true
        };
        while (true)
        {
            if (timerFiring > 0 && isFiring)
            {
                List<Collider2D> colliders = new List<Collider2D>();
                fireCollider.OverlapCollider(filter, colliders);
                foreach(Collider2D collider in colliders)
                {
                    if (collider.gameObject.TryGetComponent(out ModuleHp moduleHp))
                    {
                        if (moduleHp.transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject)
                            if (NetworkManagerCustom.singleton.gameMode.CanDamageModule(moduleHp, damageInfo))
                            {
                                if (collider.gameObject.TryGetComponent(out EffectModule effectModule))
                                    effectModule.AddEffects(damageInfo.effects.Select(e => e.Create()));

                                moduleHp.Damage(damageInfo);
                            }
                    }
                }
            }
            yield return new WaitForSeconds(damageSpeed);
        }    
    }
}

