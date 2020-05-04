using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class TeamStateMessage : GameMessage {

    public TeamStateMessage() { }

    public TeamStateMessage(int teamCount, List<int> teamSlots, List<int> observers, List<List<int>> teams) {
        Writer.Write(teamCount);
        Writer.Write(new IntegerListMessage(teamSlots));
        Writer.Write(new IntegerListMessage(observers));
        for (int i = 0; i < teamCount; i++)
            Writer.Write(new IntegerListMessage(teams[i]));
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        LobbyServerTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyServerTeamGui>();
        new TeamStateMessage(
            gui.TeamCount, 
            gui.TeamSlots.ToList(), 
            gui.Observers.Select(c => Players.GetPlayer(c).Id).ToList(), 
            gui.Teams.Select(l => l.Select(c => Players.GetPlayer(c).Id).ToList()).ToList()
        ).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        LobbyClientTeamGui gui = NetworkManagerCustom.lobbyManager.GetComponent<LobbyClientTeamGui>();
        int teamCount = reader.ReadInt32();
        List<int> teamSlots = reader.ReadMessage<IntegerListMessage>().Value;
        List<int> observers = reader.ReadMessage<IntegerListMessage>().Value;
        List<List<int>> teams = new List<List<int>>();

        for (int i = 0; i < teamCount; i++) 
            teams.Add(reader.ReadMessage<IntegerListMessage>().Value);

        gui.TeamCount = teamCount;
        gui.TeamSlots = teamSlots.ToArray();
        gui.Observers = observers;
        gui.Teams = teams;
    }
}