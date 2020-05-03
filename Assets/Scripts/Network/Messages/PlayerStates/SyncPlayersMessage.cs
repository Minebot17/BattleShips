using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class SyncPlayersMessage : GameMessage {

    public SyncPlayersMessage() { }

    public SyncPlayersMessage(List<int> players, int clientId) {
        Writer.Write(new IntegerListMessage(players));
        Writer.Write(clientId);
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        Players.playerRequestPlayersEvent.CallListners(new Players.PlayerRequestPlayersEvent(conn));
        new SyncPlayersMessage(Players.All.Select(s => s.Id).ToList(), Players.GetPlayer(conn).Id).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        List<int> ids = reader.ReadMessage<IntegerListMessage>().Value;
        int clientId = reader.ReadInt32();
        Players.ClientId = clientId;
        ids.ForEach(id => Players.AddPlayer(id));
    }
}