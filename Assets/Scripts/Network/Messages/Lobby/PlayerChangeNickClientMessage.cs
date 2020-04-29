using UnityEngine.Networking;

public class PlayerChangeNickClientMessage : GameMessage {

    public PlayerChangeNickClientMessage() { }

    public PlayerChangeNickClientMessage(int id, string nick) {
        Writer.Write(id);
        Writer.Write(nick);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        string nick = reader.ReadString();
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        gui.ChangeNick(id, nick);
    }

    public override bool WithServersClient() {
        return false;
    }
}