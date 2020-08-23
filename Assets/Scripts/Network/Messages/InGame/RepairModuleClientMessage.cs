using UnityEngine;
using UnityEngine.Networking;

public class RepairModuleClientMessage : GameMessage {

    public RepairModuleClientMessage() { }

    public RepairModuleClientMessage(NetworkIdentity identity, int childIndex) {
        Writer.Write(identity);
        Writer.Write(childIndex);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        Transform cell = identity.transform.GetChild(reader.ReadInt32());
        cell.GetComponent<PolygonCollider2D>().enabled = true;
        cell.GetChild(0).gameObject.SetActive(true);
        cell.GetChild(0).gameObject.GetComponent<IDeath>().Repair();
    }
}