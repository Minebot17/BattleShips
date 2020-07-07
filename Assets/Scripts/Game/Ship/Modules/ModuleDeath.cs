using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ModuleDeath : MonoBehaviour, IDeath {
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        Destroy(transform.parent.GetComponent<BoxCollider2D>());
        Destroy(gameObject);
        if (NetworkManagerCustom.singleton.IsServer)
            transform.parent.parent.GetComponent<ShipServerController>().OnModuleDeath(damageInfo, gameObject);
    }

    public bool IsDead() {
        return isDead;
    }
}
