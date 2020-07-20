using UnityEngine;
using UnityEngine.Networking;

public class CoreModule : AbstractModule, IOnModuleDeathServer {
    public void OnModuleDeath(DamageInfo damageInfo) {
        NetworkIdentity identity = gameObject.transform.parent.parent.GetComponent<NetworkIdentity>();
        NetworkIdentity killerIdentity = damageInfo.OwnerShip;
        NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
    }
}
