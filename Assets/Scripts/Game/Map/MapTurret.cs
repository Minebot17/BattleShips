using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class MapTurret : AbstractModule {
    [SerializeField] int coolDown;
    [SerializeField] protected int damage;
    [SerializeField] float ammoSpeed;
    [SerializeField] GameObject ammoPrefab;
    [SerializeField] int bulletCount;
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform forwardPointer;
    
    protected BulletInfo bulletInfo;
    int timerCoolDown;

    protected override void Start() {
        base.Start();
        bulletInfo = new BulletInfo(damage, null) {
            effects = GetComponents<IModuleEffect>().ToList()
        };
    }

    public void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        if (rotateSpeed != 0)
            transform.localEulerAngles += new Vector3(0, 0, rotateSpeed);

        if (timerCoolDown > 0)
            timerCoolDown--;
        else {
            for (float i = -(bulletCount - 1) / 2; i <= bulletCount / 2; i++) {
                Vector2 newVec = Quaternion.Euler(0, 0, i * 10) * (forwardPointer.position - transform.position).ToVector2().normalized;
                GameObject ammo = Instantiate(ammoPrefab);
                ammo.transform.position = transform.position.ToVector2() + newVec / 2f;
                ammo.transform.position += new Vector3(0, 0, -0.2f);
                ammo.GetComponent<AbstractAmmo>().Initialize(bulletInfo, newVec * ammoSpeed);
                NetworkServer.Spawn(ammo);
            }
            timerCoolDown = (int) (coolDown * effectModule.freezeK);
        }
    }
}
