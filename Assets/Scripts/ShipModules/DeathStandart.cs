using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class DeathStandart : MonoBehaviour, IDeath {

    private bool isDead;
    
    public void OnDead(BulletInfo bulletInfo) {
        if (IsDead())
            return;
            
        isDead = true;
        ExplosionManager.moduleSmallExplosion.Explode(transform.position.ToVector2(), bulletInfo.OwnerShip); // TODO
        Destroy(transform.parent.GetComponent<BoxCollider2D>());
        Destroy(gameObject);
        transform.parent.parent.GetComponent<ShipServerController>().OnModuleDeath(bulletInfo, gameObject);
    }

    public bool IsDead() {
        return isDead;
    }
}
