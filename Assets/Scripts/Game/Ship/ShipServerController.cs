using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ShipServerController : NetworkBehaviour {
    private IGunModule[] guns = null;
    private NetworkIdentity identity;
    private ShipController commonController;

    private int initialModulesCount;
    private int currentModulesCount;
    private bool isDead = false;

    private void Start() {
        identity = GetComponent<NetworkIdentity>();
        if (!isServer)
            return;
        
        guns = GetComponentsInChildren<IGunModule>();
        initialModulesCount = GetComponentsInChildren<ModuleHp>().Length;
        currentModulesCount = initialModulesCount;
        commonController = GetComponent<ShipController>();
    }

    private void FixedUpdate() {
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
        if (module.name.Equals("AICoreModule")) {
            isDead = true;
            NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
        }
        
        new DestroyModuleClientMessage(identity, module.transform.parent.GetSiblingIndex()).SendToAllClient();
    }
}
