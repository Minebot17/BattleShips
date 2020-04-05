using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipServerController : NetworkBehaviour {
    
    private IGunModule[] guns = null;
    private NetworkIdentity identity;
    
    private int initialModulesCount;
    private int currentModulesCount;
    
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
            Vector2 shootVector = NetworkManagerCustom.singleton.playerGunVectors[identity];
            if (shootVector != Vector2.zero)
                guns[i].Shoot(shootVector.normalized);
        }
    }

    public void OnModuleDeath(string type) {
        NetworkConnection playerConnection = identity.clientAuthorityOwner;
        if (type.Equals("AICore")) {
            NetworkManagerCustom.singleton.GameOver(playerConnection);
            return;
        }

        currentModulesCount--;
        if (currentModulesCount*5 <= initialModulesCount)
            NetworkManagerCustom.singleton.GameOver(playerConnection);
    }
}
