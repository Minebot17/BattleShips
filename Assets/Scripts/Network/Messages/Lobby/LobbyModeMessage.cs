using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyModeMessage : GameMessage {

    public LobbyModeMessage() { }

    public LobbyModeMessage(List<string> startArguments) {
        Writer.Write(new StringListMessage(startArguments));
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        new LobbyModeMessage(NetworkManagerCustom.singleton.StartArguments).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        // TODO пизда расширяемости игровых режимов из-за этого свича
        NetworkManagerCustom.singleton.StartArguments = reader.ReadMessage<StringListMessage>().Value;
        string mode = NetworkManagerCustom.singleton.StartArguments.Find(s => s.StartsWith("gamemode:")).Split(':')[1];
        GameObject lobbyManager = GameObject.Find("LobbyManager");
		
        switch (mode) {
            case "ffa":
                if (NetworkManagerCustom.singleton.IsServer)
                    lobbyManager.AddComponent<LobbyServerGui>();
                else
                    lobbyManager.AddComponent<LobbyClientGui>();
                break;
            case "commands":
                if (NetworkManagerCustom.singleton.IsServer)
                    lobbyManager.AddComponent<LobbyServerTeamGui>();
                else
                    lobbyManager.AddComponent<LobbyClientTeamGui>();
                break;
        }
    }
}