using UnityEngine.Networking;

public class AddPlayerClientMessage : GameMessage {

    public AddPlayerClientMessage() { }

    public AddPlayerClientMessage(int id) {
        Writer.Write(id);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        Players.AddPlayer(reader.ReadInt32());
    }

    public override bool WithServersClient() {
        return false;
    }
}