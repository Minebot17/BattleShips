using UnityEngine;
using UnityEngine.Networking;

public class MapElementDeath : MonoBehaviour, IDeath {

    [SerializeField] private bool destroyParent;
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        Invoke(nameof(Death), 0.15f);
    }

    private void Death() {
        Destroy(gameObject);
        if (destroyParent)
            NetworkServer.Destroy(transform.parent.gameObject);
    }

    public bool IsDead() {
        return isDead;
    }

    public void Repair() {
        isDead = false;
    }
}
