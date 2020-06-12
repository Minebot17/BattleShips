using UnityEngine;
using UnityEngine.Networking;

public class CommonGunModule : AbstractGunModule {

    [SerializeField] private float ammoSpeed;
    [SerializeField] private GameObject ammoPrefab;

    public override void Shoot(Vector2 vec) {
        GameObject ammo = Instantiate(ammoPrefab);
        ammo.transform.position = transform.position.ToVector2() + vec / 2f;
        ammo.transform.position += new Vector3(0, 0, -0.2f);
        ammo.GetComponent<IAmmo>().Initialize(transform.parent.parent.gameObject, (vec * ammoSpeed));
        NetworkServer.Spawn(ammo);
    }
}