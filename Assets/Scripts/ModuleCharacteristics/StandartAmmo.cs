using System;
using UnityEngine;
using UnityEngine.Networking;

public class StandartAmmo : NetworkBehaviour, IAmmo {

    [SerializeField] 
    private int damage;

    [SerializeField] 
    private int lifeSpan;
    
    private int lifeSpanTimer = 999999;
    private GameObject owner;

    public void Init(GameObject owner, Vector2 shootVector) {
        this.owner = owner;
        GetComponent<Rigidbody2D>().AddForce(shootVector, ForceMode2D.Impulse);
        lifeSpanTimer = lifeSpan;
    }

    public void OnCollide(ModuleHp hp) {
        if (!isServer)
            return;

        if (hp.gameObject.transform.parent.parent.gameObject != owner) {
            hp.Damage(damage);
            NetworkServer.Destroy(gameObject);
        }
    }

    private void FixedUpdate() {
        if (!isServer)
            return;

        if (lifeSpanTimer <= 0)
            NetworkServer.Destroy(gameObject);
        else
            lifeSpanTimer--;
    }
}
