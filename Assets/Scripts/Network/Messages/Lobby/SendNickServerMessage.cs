using UnityEngine.Networking;

public class SendNickServerMessage : GameMessage {

    public SendNickServerMessage() { }

    public SendNickServerMessage(string nick) {
        Writer.Write(nick);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        string nick = reader.ReadString();
        Players.GetPlayer(conn).GetState<GameState>().Nick.Value = nick.Equals("ip") ? conn.address : nick;
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}