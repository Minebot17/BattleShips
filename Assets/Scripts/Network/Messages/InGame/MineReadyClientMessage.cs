using UnityEngine;
using UnityEngine.Networking;

public class MineReadyClientMessage : GameMessage {

    public MineReadyClientMessage() { }

    public MineReadyClientMessage(NetworkIdentity identity) {
        Writer.Write(identity);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        reader.ReadNetworkIdentity().GetComponent<SpriteRenderer>().color = Color.red;
    }
}