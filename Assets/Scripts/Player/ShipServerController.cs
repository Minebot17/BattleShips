using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ShipServerController : NetworkBehaviour {

    IGunModule[] guns = null;
    NetworkIdentity identity;
    ShipController commonController;
    
    int initialModulesCount;
    int currentModulesCount;
    bool isDead = false;

    void Start() {
        identity = GetComponent<NetworkIdentity>();
        if (!isServer)
            return;
        
        guns = GetComponentsInChildren<IGunModule>();
        initialModulesCount = GetComponentsInChildren<ModuleHp>().Length;
        currentModulesCount = initialModulesCount;
        commonController = GetComponent<ShipController>();
    }

    void FixedUpdate() {
        if (!isServer || !identity)
            return;

        for (int i = 0; i < guns.Length; i++) {
            if (!(UnityEngine.Object)guns[i])
                continue;
            
            if (Players.GetPlayer(identity.clientAuthorityOwner).GetState<CommonState>().IsShoot.Value)
                guns[i].TryShoot(commonController.GetForward());
        }
    }

    public void OnModuleDeath(DamageInfo damageInfo, GameObject module) {
        if (isDead)
            return;

        currentModulesCount--;
        NetworkIdentity killerIdentity = damageInfo.OwnerShip;
        if (module.name.Equals("AICoreModule") ||
            currentModulesCount * (100/NetworkManagerCustom.percentToDeath) <= initialModulesCount ||
            (guns.Count(g => (UnityEngine.Object)g) == 1 && (UnityEngine.Object)module.GetComponent<AbstractGunModule>())) {
            isDead = true;
            NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
        }
        
        new DestroyModuleClientMessage(identity, module.transform.parent.GetSiblingIndex()).SendToAllClient();
    }
}
