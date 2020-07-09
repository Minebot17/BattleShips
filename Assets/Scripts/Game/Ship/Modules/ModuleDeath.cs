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
        if (NetworkManagerCustom.singleton.IsServer) {
            NetworkIdentity identity = gameObject.transform.parent.parent.GetComponent<NetworkIdentity>();
            IOnModuleDeathServer[] serverDeathArray = gameObject.GetComponents<IOnModuleDeathServer>();
            foreach (IOnModuleDeathServer serverDeath in serverDeathArray)
                serverDeath.OnModuleDeath(damageInfo);

            new DestroyModuleClientMessage(identity, gameObject.transform.parent.GetSiblingIndex()).SendToAllClient();
        }
    }

    public bool IsDead() {
        return isDead;
    }
}
