using UnityEngine.Networking;

public class RemovePlayerStateClientMessage : GameMessage {

    public RemovePlayerStateClientMessage() { }

    public RemovePlayerStateClientMessage(int id) {
        Writer.Write(id);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        Players.RemovePlayer(reader.ReadInt32());
    }

    public override bool WithServersClient() {
        return false;
    }
}