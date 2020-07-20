using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ModuleDeath : MonoBehaviour, IDeath {
    private bool isDead;

    public void OnDead(DamageInfo damageInfo) {
        if (IsDead())
            return;

        isDead = true;
        if (NetworkManagerCustom.singleton.IsServer) {
            NetworkIdentity identity = gameObject.transform.parent.parent.GetComponent<NetworkIdentity>();
            IOnModuleDeathServer[] serverDeathArray = gameObject.GetComponents<IOnModuleDeathServer>();
            foreach (IOnModuleDeathServer serverDeath in serverDeathArray)
                serverDeath.OnModuleDeath(damageInfo);

            if (!GetComponent<RepairModule>())
                transform.parent.parent.GetComponent<ShipServerController>().killedModules.Add(gameObject);
            new DestroyModuleClientMessage(identity, gameObject.transform.parent.GetSiblingIndex()).SendToAllClient();
        }
        
        transform.parent.GetComponent<BoxCollider2D>().enabled = false;
        gameObject.SetActive(false);
    }

    public bool IsDead() {
        return isDead;
    }

    public void Repair() {
        isDead = false;
    }
}
