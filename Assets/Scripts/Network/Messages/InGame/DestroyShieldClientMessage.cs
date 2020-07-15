using UnityEngine;
using UnityEngine.Networking;

public class DestroyShieldClientMessage : GameMessage {

    public DestroyShieldClientMessage() { }
    
    public DestroyShieldClientMessage(NetworkIdentity identity) {
        Writer.Write(identity);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        identity.transform.Find("ShieldRenderer").GetChild(0).GetComponent<ShieldDeath>().OnDead(null);
    }
}