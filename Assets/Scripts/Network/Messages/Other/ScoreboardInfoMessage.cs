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
        IEnumerable<GameState> gStates = Players.GetStates<GameState>();
        new ScoreboardInfoMessage(
        gStates.Select(d => d.ShipJson.Value).ToList(),
        gStates.Select(d => d.Score.Value).ToList(),
            NetworkManagerCustom.singleton.gameMode.GetScoreDelta(gStates.ToDictionary(d => d.GetParent().Conn, d => d.Kills.Value)).Values.ToList(),
            NetworkManagerCustom.singleton.scoreForWin
        ).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        Scoreboard.singleton.Initialize(
            reader.ReadMessage<StringListMessage>().Value,
            reader.ReadMessage<IntegerListMessage>().Value,
            reader.ReadMessage<IntegerListMessage>().Value,
            reader.ReadInt32()
        );
    }
}