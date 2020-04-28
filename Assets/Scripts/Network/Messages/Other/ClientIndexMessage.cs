using UnityEngine.Networking;

public class ClientIndexMessage : GameMessage {

    public ClientIndexMessage() { }

    public ClientIndexMessage(int index) {
        Writer.Write(index);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        new ClientIndexMessage(NetworkManagerCustom.singleton.playerData[conn].Id).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkManagerCustom.singleton.clientIndex = reader.ReadInt32();
    }
}