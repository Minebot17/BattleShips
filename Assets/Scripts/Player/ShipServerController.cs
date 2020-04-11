using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipServerController : NetworkBehaviour {
    
    private IGunModule[] guns = null;
    private NetworkIdentity identity;
    
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
    }

    private void FixedUpdate() {
        if (!isServer || !NetworkManagerCustom.singleton.playerGunVectors.ContainsKey(identity))
            return;

        for (int i = 0; i < guns.Length; i++) {
            if (!(UnityEngine.Object)guns[i])
                continue;
            
            Vector2 shootVector = NetworkManagerCustom.singleton.playerGunVectors[identity];
            if (shootVector != Vector2.zero)
                guns[i].Shoot(shootVector.normalized);
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
