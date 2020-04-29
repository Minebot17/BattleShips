using UnityEngine.Networking;

public class ChangeTeamCountClientMessage : GameMessage {

    public ChangeTeamCountClientMessage() { }

    public ChangeTeamCountClientMessage(int newTeamCount) {
        Writer.Write(newTeamCount);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        int newTeamCount = reader.ReadInt32();
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        gui.ChangeTeamCount(newTeamCount);
    }
    
    public override bool WithServersClient() {
        return false;
    }
}