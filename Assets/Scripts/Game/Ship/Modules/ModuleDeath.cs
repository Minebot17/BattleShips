using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ModuleDeath : MonoBehaviour, IDeath {
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        ExplosionManager.moduleSmallExplosion.Explode(transform.position.ToVector2(), damageInfo.OwnerShip);
        Destroy(transform.parent.GetComponent<BoxCollider2D>());
        Destroy(gameObject);
        transform.parent.parent.GetComponent<ShipServerController>().OnModuleDeath(damageInfo, gameObject);
    }

    public bool IsDead() {
        return isDead;
    }
}
