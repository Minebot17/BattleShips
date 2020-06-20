using UnityEngine;
using UnityEngine.Networking;

public class MapElementDeath : MonoBehaviour, IDeath {

    [SerializeField]
    bool destroyParent;
    bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        Destroy(gameObject);
        if (destroyParent)
            NetworkServer.Destroy(transform.parent.gameObject);
    }

    public bool IsDead() {
        return isDead;
    }
}
