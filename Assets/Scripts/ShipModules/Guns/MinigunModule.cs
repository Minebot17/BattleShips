
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

class MinigunModule : AbstractGunModule
{
    [SerializeField] float startCoolDown;
    [SerializeField] float ammoSpeed;
    [SerializeField] GameObject ammoPrefab;
    [SerializeField] protected float timeToFullWarming;

    float timerWarming;
    private float timeLastShoot;

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (timerWarming < timeToFullWarming && timerCoolDown <= 0)
            timerWarming += Time.fixedDeltaTime / 2;
    }

    protected override void Shoot(Vector2 vec)
    {
        GameObject ammo = Instantiate(ammoPrefab);
        ammo.transform.position = transform.position.ToVector2() + vec / 2f;
        ammo.transform.position += new Vector3(0, 0, -0.2f);
        ammo.GetComponent<AbstractAmmo>().Initialize(damageInfo, (vec * ammoSpeed));

        NetworkServer.Spawn(ammo);
        
        timerCoolDown = (coolDown + (startCoolDown - coolDown) * (timerWarming / timeToFullWarming)) * effectModule.freezeK;
        timerWarming = timerWarming - Time.time + timeLastShoot > 0 ? timerWarming - Time.time + timeLastShoot : 0;
        timeLastShoot = Time.time;
    }
}

