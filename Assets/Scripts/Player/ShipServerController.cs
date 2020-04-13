using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

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
        if (!isServer || !NetworkManagerCustom.singleton.playersGunButton.ContainsKey(identity))
            return;

        for (int i = 0; i < guns.Length; i++) {
            if (!(UnityEngine.Object)guns[i])
                continue;
            
            if (NetworkManagerCustom.singleton.playersGunButton[identity])
                guns[i].Shoot(commonController.GetForward());
        }
    }

    public void OnModuleDeath(DamageSource damageSource, string type) {
        if (isDead)
            return;
        
        NetworkIdentity killerIdentity = damageSource is PlayerDamageSource pds ? pds.OwnerShip : null;
        if (type.Equals("AICore")) {
            NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
            isDead = true;
            return;
        }

        currentModulesCount--;
        if (currentModulesCount * 5 <= initialModulesCount) {
            isDead = true;
            NetworkManagerCustom.singleton.PlayerKill(killerIdentity, identity);
        }
    }
}
