using UnityEngine;
using UnityEngine.Networking;

public class DestroyModuleClientMessage : GameMessage {

    public DestroyModuleClientMessage() { }

    public DestroyModuleClientMessage(NetworkIdentity identity, int childIndex) {
        Writer.Write(identity);
        Writer.Write(childIndex);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        identity.gameObject.GetComponent<ShipController>().OnModuleDeath(identity.transform.GetChild(reader.ReadInt32()));
    }
}