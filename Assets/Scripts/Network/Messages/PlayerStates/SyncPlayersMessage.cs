using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class SyncPlayersMessage : GameMessage {

    public SyncPlayersMessage() { }

    public SyncPlayersMessage(List<int> players, int clientId, int hostId, List<string> startArguments) {
        Writer.Write(new IntegerListMessage(players));
        Writer.Write(clientId);
        Writer.Write(hostId);
        Writer.Write(new StringListMessage(startArguments));
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        Players.playerRequestPlayersEvent.CallListners(new Players.ConnectionEvent(conn));
        new SyncPlayersMessage(
            Players.All.Select(s => s.Id).ToList(), 
            Players.GetPlayer(conn).Id,
            Players.HostId,
            NetworkManagerCustom.singleton.StartArguments
        ).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        List<int> ids = reader.ReadMessage<IntegerListMessage>().Value;
        int clientId = reader.ReadInt32();
        Players.HostId = reader.ReadInt32();
        Players.ClientId = clientId;
        ids.ForEach(id => Players.AddPlayer(id));
        
        // TODO пизда расширяемости игровых режимов из-за этого свича
        NetworkManagerCustom.singleton.StartArguments = reader.ReadMessage<StringListMessage>().Value;
        string mode = NetworkManagerCustom.singleton.StartArguments.Find(s => s.StartsWith("gamemode:")).Split(':')[1];
        GameObject lobbyManager = NetworkManagerCustom.lobbyManager;
		
        switch (mode) {
            case "ffa":
                if (NetworkManagerCustom.singleton.IsServer)
                    lobbyManager.AddComponent<LobbyServerGui>();
                else {
                    NetworkManagerCustom.singleton.gameMode = new FFAGameMode();
                    lobbyManager.AddComponent<LobbyClientGui>();
                }
                break;
            case "commands":
                if (NetworkManagerCustom.singleton.IsServer)
                    lobbyManager.AddComponent<LobbyServerTeamGui>();
                else {
                    NetworkManagerCustom.singleton.gameMode = new TeamGameMode(null); // TODO а еще вот это костыль, по нормальному должен присваиваться сразу после начала игры
                    lobbyManager.AddComponent<LobbyClientTeamGui>();
                }
                break;
        }
    }
}