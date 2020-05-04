using UnityEngine.Networking;

public class RemovePlayerClientMessage : GameMessage {

    public RemovePlayerClientMessage() { }

    public RemovePlayerClientMessage(int id) {
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