using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class DeathStandart : MonoBehaviour, IDeath {
    public void OnDead(DamageSource source) {
        Destroy(gameObject);
        Destroy(transform.parent.GetComponent<BoxCollider2D>());
        transform.parent.parent.GetComponent<ShipServerController>().OnModuleDeath(source, gameObject);
    }
}
