using System.Text.RegularExpressions;
using UnityEngine.Networking;

public class SendNickServerMessage : GameMessage {

    public static Regex nickTemplate = new Regex("([a-zA-Z0-9.]){1,16}"); 

    public SendNickServerMessage() { }

    public SendNickServerMessage(string nick) {
        Writer.Write(nick);
    }
    public SendNickServerMessage(bool validNick)
    {
        Writer.Write(validNick);
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        string nick = reader.ReadString();
        new SendNickServerMessage(nickTemplate.IsMatch(nick)).SendToClient(conn);
        NetworkManagerCustom.singleton.playerData[conn].Nick = nick.Equals("ip") ? conn.address : nick;
    }
    
    public override void OnClient(NetworkReader reader) {
        bool valid = reader.ReadBoolean();
        LobbyClientGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientGui>();
        gui.SetNick(valid);
    }

}