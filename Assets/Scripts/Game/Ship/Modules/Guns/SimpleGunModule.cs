using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleGunModule : AbstractGunModule {
    [SerializeField] private float ammoSpeed;
    [SerializeField] private GameObject ammoPrefab;
    [SerializeField] private int bulletCount;

    protected override void Shoot(Vector2 vec) {
        for (float i = -(bulletCount - 1) / 2; i <= bulletCount / 2; i++) {
            Vector2 newVec = Quaternion.Euler(0, 0, i * 10) * vec;
            GameObject ammo = Instantiate(ammoPrefab);
            ammo.transform.position = transform.position.ToVector2() + newVec / 2f;
            ammo.transform.position += new Vector3(0, 0, -0.2f);
            ammo.GetComponent<AbstractAmmo>().Initialize(damageInfo, (newVec * ammoSpeed));

            NetworkServer.Spawn(ammo);
        }
    }
}