using UnityEngine;
using UnityEngine.Networking;

public class AddTeamPlayerClientMessage : GameMessage {

    public AddTeamPlayerClientMessage() { }

    public AddTeamPlayerClientMessage(int id) {
        Writer.Write(id);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        gui.AddPlayer(id);
    }
    
    public override bool WithServersClient() {
        return false;
    }
}