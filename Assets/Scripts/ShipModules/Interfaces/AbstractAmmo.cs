using UnityEngine;
using UnityEngine.Networking;

public abstract class AbstractAmmo : NetworkBehaviour
{
    [SerializeField]
    protected int lifeSpan;
    [SerializeField]
    protected int damage;

    protected GameObject owner;

    private NetworkIdentity ownerIdentity;

    protected int lifeSpanTimer = 999999;

    virtual public void Initialize(GameObject owner, Vector2 shootVector)
    {
        this.owner = owner;
        ownerIdentity = owner.GetComponent<NetworkIdentity>();
        lifeSpanTimer = lifeSpan;
    }

    abstract public void OnCollide(ModuleHp hp);

    public DamageSource GetDamageSource()
    {
        return new PlayerDamageSource(damage, ownerIdentity);
    }

    private void FixedUpdate()
    {
        if (!isServer)
            return;

        if (lifeSpanTimer <= 0)
            NetworkServer.Destroy(gameObject);
        else
            lifeSpanTimer--;
    }
}
