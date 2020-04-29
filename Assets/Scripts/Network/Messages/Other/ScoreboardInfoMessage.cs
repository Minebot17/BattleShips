using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class ScoreboardInfoMessage : GameMessage {

    public ScoreboardInfoMessage() { }

    public ScoreboardInfoMessage(List<string> jsons, List<int> score, List<int> kills, int scoreForWin) {
        Writer.Write(new StringListMessage(jsons));
        Writer.Write(new IntegerListMessage(score));
        Writer.Write(new IntegerListMessage(kills));
        Writer.Write(scoreForWin);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        new ScoreboardInfoMessage(
            NetworkManagerCustom.singleton.playerData.Values.Select(d => d.ShipJson).ToList(),
            NetworkManagerCustom.singleton.playerData.Values.Select(d => d.Score).ToList(),
            NetworkManagerCustom.singleton.gameMode.GetScoreDelta(NetworkManagerCustom.singleton.playerData.ToDictionary(d => d.Key, d => d.Value.Kills)).Values.ToList(),
            NetworkManagerCustom.singleton.scoreForWin
        ).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        Scoreboard.singleton.Init(
            reader.ReadMessage<StringListMessage>().Value,
            reader.ReadMessage<IntegerListMessage>().Value,
            reader.ReadMessage<IntegerListMessage>().Value,
            reader.ReadInt32()
        );
    }
}