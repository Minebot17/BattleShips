using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyServerTeamGui : LobbyServerGui {
    
    protected List<NetworkConnection> observers = new List<NetworkConnection>();
    protected List<List<NetworkConnection>> teams = new List<List<NetworkConnection>>() { new List<NetworkConnection>(), new List<NetworkConnection>() };
    protected string teamCountStr = "2";
    protected const int MaxTeamCount = 12;
    protected int teamCount = 2;
    protected string[] teamSlotsStr = { "2", "2" };
    protected int[] teamSlots = { 2, 2 };

    public List<NetworkConnection> Observers => observers;
    public List<List<NetworkConnection>> Teams => teams;
    public int TeamCount => teamCount;
    public int[] TeamSlots => teamSlots;

    protected override void Start() {
        base.Start();
        observers.AddRange(Players.Conns);

        Players.playerAddedEvent.SubcribeEvent(e => {
            observers.Add(e.Player.Conn);
            new AddTeamPlayerClientMessage(e.Player.Id).SendToAllClient(e.Player.Conn);
        });

        Players.playerRemovedEvent.SubcribeEvent(e => {
            int teamIndex = FindTeamOfConnection(e.Player.Conn);
            if (teamIndex == -1)
                observers.Remove(e.Player.Conn);
            else 
                teams[teamIndex].Remove(e.Player.Conn);
            new RemoveTeamPlayerClientMessage(e.Player.Id).SendToAllClient();
        });
    }
    
    protected override void RenderInChild() {
		GUILayout.Space(10);
        GUILayout.Label("Кол-во команд:");
        teamCountStr = GUILayout.TextField(teamCountStr);
        if (GUILayout.Button("OK")) {
            teamCount = int.TryParse(teamCountStr, out teamCount) ? teamCount : MaxTeamCount;
            
            if (teamSlots.Length > teamCount) {
                for (int i = teamCount; i < teamSlots.Length; i++) {
                    foreach (NetworkConnection conn in teams[i])
                        ChangeTeam(conn, i, -1);
                }
                
                teams.RemoveRange(teamCount, teamSlots.Length - teamCount);
            }
            else if (teamSlots.Length < teamCount) {
                for (int i = 0; i < teamCount - teamSlots.Length; i++)
                    teams.Add(new List<NetworkConnection>());
            }

            int minSlots = teamCount < teamSlots.Length ? teamCount : teamSlots.Length;
            
            string[] newSlotsStr = new string[teamCount];
            for (int i = 0; i < minSlots; i++)
                newSlotsStr[i] = teamSlotsStr[i];
            teamSlotsStr = newSlotsStr;
            for (int i = minSlots; i < teamSlotsStr.Length; i++)
                teamSlotsStr[i] = "2";
            
            int[] newSlots = new int[teamCount];
            for (int i = 0; i < minSlots; i++)
                newSlots[i] = teamSlots[i];
            teamSlots = newSlots;
            for (int i = minSlots; i < teamSlots.Length; i++)
                teamSlots[i] = 2;
            
            new ChangeTeamCountClientMessage(teamCount).SendToAllClient();
        }

        GUILayout.Space(10);
        GUILayout.Label("Наблюдатели:");
        foreach (NetworkConnection conn in observers)
            GUILayout.Label("* " + Players.GetPlayer(conn).GetState<GameState>().Nick.Value
                            + " (" + (Players.GetPlayer(conn).GetState<LobbyState>().Ready.Value ? "Готов" : "Не готов") + ")");
        if (GUILayout.Button("Перейти"))
            ChangeTeam(-1);

        for (int i = 0; i < teamCount; i++) {
            List<NetworkConnection> team = teams[i];
            GUILayout.Space(10);
            GUILayout.Label("Команда #" + i);
            GUILayout.Label("Слотов:");
            teamSlotsStr[i] = GUILayout.TextField(teamSlotsStr[i]);
            if (GUILayout.Button("OK")) {
                int oldSlots = teamSlots[i];
                teamSlots[i] = int.Parse(teamSlotsStr[i]);
                if (oldSlots > teamSlots[i] && team.Count > teamSlots[i]) {
                    for (int j = teamSlots[i]; j < team.Count; j++) {
                        ChangeTeam(team[j], i, -1);
                    }
                }
                
                new ChangeTeamSlotsClientMessage(i, teamSlots[i]).SendToAllClient();
            }
            
            IEnumerable<string> nicksInTeam = from player in Players.All 
                                              where team.Contains(player.Conn) 
                                              select player.GetState<GameState>().Nick.Value 
                                                     + " (" + (player.GetState<LobbyState>().Ready.Value ? "Готов" : "Не готов") + ")";

            foreach (string nick in nicksInTeam) 
                GUILayout.Label("* " + nick);

            if (teams[i].Count < teamSlots[i] && GUILayout.Button("Перейти"))
                ChangeTeam(i);
        }
    }
    
    protected override void OnStartGame() {
        NetworkManagerCustom.singleton.gameMode = new TeamGameMode(teams);
    }

    void ChangeTeam(int to) {
        ChangeTeam(Players.GetClient().Conn, to);
    }

    public void ChangeTeam(NetworkConnection conn, int to) {
        int currentTeam = FindTeamOfConnection(conn);
        ChangeTeam(conn, currentTeam, to);
    }

    void ChangeTeam(NetworkConnection conn, int from, int to) {
        List<NetworkConnection> fromList = from == -1 ? observers : teams[from];
        List<NetworkConnection> toList = to == -1 ? observers : teams[to];
        fromList.Remove(conn);
        toList.Add(conn);
        new ChangeTeamMessage(Players.GetPlayer(conn).Id, from, to).SendToAllClient();
    }

    int FindTeamOfConnection(NetworkConnection conn) {
        if (observers.Contains(conn))
            return -1;
        
        for (int i = 0; i < teams.Count; i++)
            if (teams[i].Contains(conn))
                return i;

        return -1;
    }
}
