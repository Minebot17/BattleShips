using UnityEngine.Networking;

public class ChangeTeamSlotsClientMessage : GameMessage {

    public ChangeTeamSlotsClientMessage() { }

    public ChangeTeamSlotsClientMessage(int team, int teamSlots) {
        Writer.Write(team);
        Writer.Write(teamSlots);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        int team = reader.ReadInt32();
        int teamSlots = reader.ReadInt32();
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        gui.ChangeTeamSlots(team, teamSlots);
    }
    
    public override bool WithServersClient() {
        return false;
    }
}