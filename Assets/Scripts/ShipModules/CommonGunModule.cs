using System;
using UnityEngine;
using UnityEngine.Networking;

public class CommonGunModule : AbstractGunModule
{
    [SerializeField]
    private float ammoSpeed;

    [SerializeField]
    private GameObject ammoPrefab;

    [SerializeField]
    private int bulletCount;

    protected override void Shoot(Vector2 vec) {
        for(float i = -(bulletCount - 1) / 2; i <= bulletCount / 2; i++)
        {
            Vector2 newVec = vec.Rotate(i * 10);
            GameObject ammo = Instantiate(ammoPrefab);
            ammo.transform.position = transform.position.ToVector2() + newVec / 2f;
            ammo.transform.position += new Vector3(0, 0, -0.2f);
            ammo.GetComponent<AbstractAmmo>().Initialize(transform.parent.parent.gameObject, (newVec * ammoSpeed));

            NetworkServer.Spawn(ammo);
        }      
    }
}
public static class Vector2Extension
{

    public static Vector2 Rotate(this Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }
}
