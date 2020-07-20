using UnityEngine;
using UnityEngine.Networking;

public class ShieldDeath : MonoBehaviour, IDeath {
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        Destroy(transform.parent.gameObject);
        Destroy(gameObject);
        if (NetworkManagerCustom.singleton.IsServer) {
            NetworkIdentity identity = gameObject.transform.parent.parent.GetComponent<NetworkIdentity>();
            new DestroyShieldClientMessage(identity).SendToAllClientExceptHost();
        }
    }

    public bool IsDead() {
        return isDead;
    }

    public void Repair() {
        isDead = false;
    }
}
