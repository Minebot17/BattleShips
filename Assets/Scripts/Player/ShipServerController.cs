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
            
            if (NetworkManagerCustom.singleton.playerData[identity.clientAuthorityOwner].IsShoot)
                guns[i].Shoot(commonController.GetForward());
        }
    }

    public void OnModuleDeath(DamageSource damageSource, GameObject module) {
        if (isDead)
            return;

        currentModulesCount--;
        NetworkIdentity killerIdentity = damageSource is PlayerDamageSource pds ? pds.OwnerShip : null;
        if (module.name.Equals("AICoreModule") ||
            currentModulesCount * (100/NetworkManagerCustom.percentToDeath) <= initialModulesCount ||
            (guns.Count(g => (UnityEngine.Object)g) == 1 && (UnityEngine.Object)module.GetComponent<IGunModule>())) {
            isDead = true;
            NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
        }
        
        MessageManagerOld.DestroyModuleClientMessage.SendToAllClients(new MessagesMessage(new MessageBase[] {
            new NetworkIdentityMessage(identity),
            new StringMessage(module.transform.parent.gameObject.name)
        }));
    }
}
