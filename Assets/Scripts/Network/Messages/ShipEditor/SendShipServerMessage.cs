using UnityEngine.Networking;

public class SendShipServerMessage : GameMessage {

    public SendShipServerMessage() { }

    public SendShipServerMessage(string json) {
        Writer.Write(json);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkManagerCustom.singleton.playerData[conn].ShipJson = reader.ReadString();
        if (--NetworkManagerCustom.singleton.lastConnections == 0) 
            NetworkManagerCustom.singleton.ServerChangeScene("Game");
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}