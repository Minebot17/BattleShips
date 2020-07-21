using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class ShieldDeath : MonoBehaviour, IDeath {
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        StartCoroutine(DestroyShield());
        if (NetworkManagerCustom.singleton.IsServer) {
            NetworkIdentity identity = gameObject.transform.parent.parent.GetComponent<NetworkIdentity>();
            new DestroyShieldClientMessage(identity).SendToAllClientExceptHost();
        }
    }

    private IEnumerator DestroyShield() {
        yield return new WaitForSeconds(0.25f);
        Destroy(transform.parent.gameObject);
        Destroy(gameObject);
    }

    public bool IsDead() {
        return isDead;
    }

    public void Repair() {
        isDead = false;
    }
}
