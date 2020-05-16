using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class DeathStandart : MonoBehaviour, IDeath {

    private bool isDead;
    
    public void OnDead(DamageSource source) {
        if (IsDead())
            return;
            
        isDead = true;
        ExplosionManager.moduleSmallExplosion.Explode(transform.position.ToVector2()); // TODO
        Destroy(transform.parent.GetComponent<BoxCollider2D>());
        Destroy(gameObject);
        transform.parent.parent.GetComponent<ShipServerController>().OnModuleDeath(source, gameObject);
    }

    public bool IsDead() {
        return isDead;
    }
}
