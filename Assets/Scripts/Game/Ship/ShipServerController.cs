using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ShipServerController : NetworkBehaviour {
    public List<GameObject> killedModules = new List<GameObject>();
    public Dictionary<string, UsableModuleInfo> usableModules = new Dictionary<string, UsableModuleInfo>(); // key - module name, value - alive cell names and coolDown

    private IGunModule[] guns = null;
    private NetworkIdentity identity;
    private ShipController commonController;

    private void Start() {
        identity = GetComponent<NetworkIdentity>();
        if (!isServer)
            return;
        
        guns = GetComponentsInChildren<IGunModule>();
        commonController = GetComponent<ShipController>();
    }

    private void FixedUpdate() {
        if (!isServer || !identity)
            return;

        for (int i = 0; i < guns.Length; i++) {
            if (!(UnityEngine.Object)guns[i] || !((MonoBehaviour)guns[i]).gameObject.activeSelf)
                continue;
            
            if (Players.GetPlayer(identity.clientAuthorityOwner).GetState<CommonState>().IsShoot.Value)
                guns[i].TryShoot(commonController.GetForward());
        }
    }
}
