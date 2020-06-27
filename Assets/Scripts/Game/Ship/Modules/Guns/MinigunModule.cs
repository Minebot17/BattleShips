
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

internal class MinigunModule : AbstractGunModule
{
    [SerializeField] private float startCoolDown;
    [SerializeField] private float ammoSpeed;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] protected float timeToFullWarming;

    private float timerWarming;
    private float timeLastShoot;
    private float lastTimerCoolDown;

    protected override void Start() {
        base.Start();
        timerWarming = timeToFullWarming;
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (timerWarming < timeToFullWarming && timerCoolDown <= 0)
            timerWarming += Time.fixedDeltaTime;
    }

    protected override void Shoot(Vector2 vec)
    {
        GameObject ammo = Instantiate(ammoPrefab);
        ammo.transform.position = transform.position.ToVector2() + vec / 2f;
        ammo.transform.position += new Vector3(0, 0, -0.2f);
        ammo.GetComponent<AbstractAmmo>().Initialize(damageInfo, (vec * ammoSpeed));

        NetworkServer.Spawn(ammo);
        
        timerCoolDown = (coolDown + (startCoolDown - coolDown) * (timerWarming / timeToFullWarming)) * effectModule.freezeK;
        if (Time.time - timeLastShoot > lastTimerCoolDown * 1.1f)
            timeLastShoot = Time.time;
        
        lastTimerCoolDown = timerCoolDown;
        timerWarming = timerWarming - Time.time + timeLastShoot > 0 ? timerWarming - Time.time + timeLastShoot : 0;
        timeLastShoot = Time.time;
    }
}

