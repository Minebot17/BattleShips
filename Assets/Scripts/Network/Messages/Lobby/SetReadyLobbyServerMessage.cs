using UnityEngine;
using UnityEngine.Networking;

public class SetReadyLobbyServerMessage : GameMessage {

    public SetReadyLobbyServerMessage() { }

    public SetReadyLobbyServerMessage(bool ready) {
        Writer.Write(ready);
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        bool ready = reader.ReadBoolean();
        GameObject.Find("LobbyManager").GetComponent<LobbyServerGui>().SetReady(conn, ready);
    }
}
