using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LootItem : MonoBehaviour {
    
    private void Start() {
        new LootItemMessage(GetComponent<NetworkIdentity>()).SendToServer();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (!NetworkManagerCustom.singleton.IsServer 
            || !other.transform.parent 
            || !other.transform.parent.TryGetComponent(out NetworkIdentity identity)) 
            return;
        
        Player player = Players.GetPlayer(identity);
        if (player == null) 
            return;
        
        string moduleName = gameObject.name.Split('_')[1];
        for (int i = 0; i < ShipEditor.modules.Length; i++) {
            if (!ShipEditor.modules[i].name.Equals(moduleName)) 
                continue;
                    
            player.GetState<InventoryState>().modulesCount[i].Value++;
            NetworkServer.Destroy(gameObject);
            break;
        }
    }
}
