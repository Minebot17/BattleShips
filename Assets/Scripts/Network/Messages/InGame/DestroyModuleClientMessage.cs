using UnityEngine;
using UnityEngine.Networking;

public class DestroyModuleClientMessage : GameMessage {

    public DestroyModuleClientMessage() { }

    public DestroyModuleClientMessage(NetworkIdentity identity, string cellName) {
        Writer.Write(identity);
        Writer.Write(cellName);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        string cellName = reader.ReadString();
        Transform cellTransform = identity.transform.Find(cellName);
        identity.gameObject.GetComponent<ShipController>().OnModuleDeath(cellTransform);
    }
}