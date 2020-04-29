using UnityEngine;
using UnityEngine.Networking;

public class ChangeTeamMessage : GameMessage {

    public ChangeTeamMessage() { }

    public ChangeTeamMessage(int to) { // to server
        Writer.Write(to);
    }

    public ChangeTeamMessage(int id, int from, int to) { // to client
        Writer.Write(id);
        Writer.Write(from);
        Writer.Write(to);
    } 
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        int to = reader.ReadInt32();
        LobbyServerTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyServerTeamGui>();
        gui.ChangeTeam(conn, to);
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        int from = reader.ReadInt32();
        int to = reader.ReadInt32();
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        gui.ChangeTeam(id, from, to);
    }

    public override bool WithServersClient() {
        return false;
    }
}