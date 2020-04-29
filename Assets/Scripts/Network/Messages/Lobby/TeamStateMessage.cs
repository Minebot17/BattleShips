using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TeamStateMessage : GameMessage {

    public TeamStateMessage() { }

    public TeamStateMessage(int teamCount, List<int> teamSlots, List<int> observers, List<List<int>> teams, Dictionary<int, string> nicks) {
        Writer.Write(teamCount);
        Writer.Write(new IntegerListMessage(teamSlots));
        Writer.Write(new IntegerListMessage(observers));
        for (int i = 0; i < teamCount; i++)
            Writer.Write(new IntegerListMessage(teams[i]));
        Writer.Write(nicks.Count);
        foreach (KeyValuePair<int,string> pair in nicks) {
            Writer.Write(pair.Key);
            Writer.Write(pair.Value);
        }
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        LobbyServerTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyServerTeamGui>();
        new TeamStateMessage(
            gui.TeamCount, 
            gui.TeamSlots.ToList(), 
            gui.Observers.Select(c => NetworkManagerCustom.singleton.playerData[c].Id).ToList(), 
            gui.Teams.Select(l => l.Select(c => NetworkManagerCustom.singleton.playerData[c].Id).ToList()).ToList(),
            NetworkManagerCustom.singleton.playerData.ToDictionary(p => p.Value.Id, p => p.Value.Nick)
        ).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        int teamCount = reader.ReadInt32();
        List<int> teamSlots = reader.ReadMessage<IntegerListMessage>().Value;
        List<int> observers = reader.ReadMessage<IntegerListMessage>().Value;
        List<List<int>> teams = new List<List<int>>();
        Dictionary<int, string> nicks = new Dictionary<int, string>();

        for (int i = 0; i < teamCount; i++) 
            teams.Add(reader.ReadMessage<IntegerListMessage>().Value);

        int nickCount = reader.ReadInt32();
        for (int i = 0; i < nickCount; i++)
            nicks.Add(reader.ReadInt32(), reader.ReadString());

        gui.TeamCount = teamCount;
        gui.TeamSlots = teamSlots.ToArray();
        gui.Observers = observers;
        gui.Teams = teams;
        gui.Nicks = nicks;
    }
}