using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ShipServerController : NetworkBehaviour {
    
    private AbstractGunModule[] guns = null;
    private NetworkIdentity identity;
    private ShipController commonController;
    
    private int initialModulesCount;
    private int currentModulesCount;
    private bool isDead = false;

    private void Start() {
        identity = GetComponent<NetworkIdentity>();
        if (!isServer)
            return;
        
        guns = GetComponentsInChildren<AbstractGunModule>();
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
            
            if (Players.GetPlayer(identity.clientAuthorityOwner).GetState<GameState>().IsShoot.Value)
                guns[i].TryShoot(commonController.GetForward());
        }
    }

    public void OnModuleDeath(BulletInfo bulletInfo, GameObject module) {
        if (isDead)
            return;

        currentModulesCount--;
        NetworkIdentity killerIdentity = bulletInfo.OwnerShip;
        if (module.name.Equals("AICoreModule") ||
            currentModulesCount * (100/NetworkManagerCustom.percentToDeath) <= initialModulesCount ||
            (guns.Count(g => (UnityEngine.Object)g) == 1 && (UnityEngine.Object)module.GetComponent<AbstractGunModule>())) {
            isDead = true;
            NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
        }
        
        new DestroyModuleClientMessage(identity, module.transform.parent.gameObject.name).SendToAllClient();
    }
}
