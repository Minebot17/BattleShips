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

    private int connectEvent = -1;
    private int disconnectEvent = -1;
    private int changeNickEvent = -1;

    public List<NetworkConnection> Observers => observers;
    public List<List<NetworkConnection>> Teams => teams;
    public int TeamCount => teamCount;
    public int[] TeamSlots => teamSlots;

    protected override void Start() {
        base.Start();
        observers.AddRange(NetworkManagerCustom.singleton.playerData.Keys);

        connectEvent = NetworkManagerCustom.singleton.playerConnectedEvent.SubcribeEvent(e => {
            observers.Add(e.Conn);
            new AddTeamPlayerClientMessage(NetworkManagerCustom.singleton.playerData[e.Conn].Id).SendToAllClient(e.Conn);
        });
        
        disconnectEvent = NetworkManagerCustom.singleton.playerDisconnectedEvent.SubcribeEvent(e => {
            int teamIndex = FindTeamOfConnection(e.Conn);
            if (teamIndex == -1)
                observers.Remove(e.Conn);
            else 
                teams[teamIndex].Remove(e.Conn);
            new RemoveTeamPlayerClientMessage(NetworkManagerCustom.singleton.playerData[e.Conn].Id).SendToAllClient();
        });

        changeNickEvent = PlayerServerData.changeNickEvent.SubcribeEvent(e => {
            PlayerServerData sender = (PlayerServerData)e.Sender;
            new PlayerChangeNickClientMessage(sender.Id, sender.Nick).SendToAllClient();
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
            GUILayout.Label("* " + NetworkManagerCustom.singleton.playerData[conn].Nick);
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
            
            IEnumerable<string> nicksInTeam = from pair in NetworkManagerCustom.singleton.playerData 
                                              where team.Contains(pair.Key) 
                                              select pair.Value.Nick;

            foreach (string nick in nicksInTeam) 
                GUILayout.Label("* " + nick);

            if (teams[i].Count < teamSlots[i] && GUILayout.Button("Перейти"))
                ChangeTeam(i);
        }
    }
    
    protected override void OnStartGame() {
        NetworkManagerCustom.singleton.playerConnectedEvent.UnSubcribeEvent(connectEvent);
        NetworkManagerCustom.singleton.playerDisconnectedEvent.UnSubcribeEvent(disconnectEvent);
        PlayerServerData.changeNickEvent.UnSubcribeEvent(changeNickEvent);

        NetworkManagerCustom.singleton.gameMode = new TeamGameMode(teams);
    }

    private void ChangeTeam(int to) {
        NetworkConnection conn = NetworkManagerCustom.singleton.FindServerPlayer().Conn;
        ChangeTeam(conn, to);
    }

    public void ChangeTeam(NetworkConnection conn, int to) {
        int currentTeam = FindTeamOfConnection(conn);
        ChangeTeam(conn, currentTeam, to);
    }

    private void ChangeTeam(NetworkConnection conn, int from, int to) {
        List<NetworkConnection> fromList = from == -1 ? observers : teams[from];
        List<NetworkConnection> toList = to == -1 ? observers : teams[to];
        fromList.Remove(conn);
        toList.Add(conn);
        new ChangeTeamMessage(NetworkManagerCustom.singleton.playerData[conn].Id, from, to).SendToAllClient();
    }

    private int FindTeamOfConnection(NetworkConnection conn) {
        if (observers.Contains(conn))
            return -1;
        
        for (int i = 0; i < teams.Count; i++)
            if (teams[i].Contains(conn))
                return i;

        return -1;
    }
}
