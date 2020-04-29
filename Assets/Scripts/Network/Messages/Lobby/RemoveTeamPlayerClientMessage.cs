using UnityEngine.Networking;

public class RemoveTeamPlayerClientMessage : GameMessage {

    public RemoveTeamPlayerClientMessage() { }
    
    public RemoveTeamPlayerClientMessage(int id) {
        Writer.Write(id);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        gui.RemovePlayer(id);
    }
    
    public override bool WithServersClient() {
        return false;
    }
}