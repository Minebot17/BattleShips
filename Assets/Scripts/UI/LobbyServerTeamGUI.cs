using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyServerTeamGUI : LobbyServerGui {
    
    protected List<NetworkConnection> observers = new List<NetworkConnection>();
    protected List<List<NetworkConnection>> teams = new List<List<NetworkConnection>>();
    protected string teamCountStr = "2";
    protected int teamCount = 2;
    protected string[] teamSlotsStr = { "2", "2" };
    protected int[] teamSlots = { 2, 2 };

    private int connectEvent = -1;
    private int disconnectEvent = -1;
    private int changeNickEvent = -1;

    protected override void Start() {
        base.Start();
        observers.AddRange(NetworkManagerCustom.singleton.playerData.Keys);

        connectEvent = NetworkManagerCustom.singleton.playerConnectedEvent.SubcribeEvent(e => {
            observers.Add(e.Conn);
            // TODO отправить клиентам игрока
        });
        
        disconnectEvent = NetworkManagerCustom.singleton.playerDisconnectedEvent.SubcribeEvent(e => {
            int teamIndex = FindTeamOfConnection(e.Conn);
            teams[teamIndex].Remove(e.Conn);
            // TODO отправить клиентам удаление игрока
        });

        changeNickEvent = PlayerServerData.changeNickEvent.SubcribeEvent(e => {
            PlayerServerData sender = (PlayerServerData)e.Sender;
            // TODO отправить клиентам смену ника игрока
        });
    }
    
    protected override void RenderInChild() {
		GUILayout.Space(20);
        
        teamCountStr = GUILayout.TextField(teamCountStr);
        if (GUILayout.Button("OK")) {
            teamCount = int.Parse(teamCountStr);
            
            if (teamSlots.Length > teamCount) {
                for (int i = teamCount; i < teamSlots.Length; i++) {
                    foreach (NetworkConnection conn in teams[i])
                        ChangeTeam(conn, i, -1);
                }
                
                teams.RemoveRange(teamCount, teamSlots.Length - teamCount);
            }
            else if (teamSlots.Length < teamCount) {
                List<List<NetworkConnection>> toAdd = new List<List<NetworkConnection>>();
                for (int i = 0; i < teamCount - teamSlots.Length; i++)
                    toAdd.Add(new List<NetworkConnection>());
                
                teams.AddRange(toAdd);
            }

            string[] newSlotsStr = new string[teamCount];
            for (int i = 0; i < teamCount; i++)
                newSlotsStr[i] = teamSlotsStr[i];
            teamSlotsStr = newSlotsStr;
            
            int[] newSlots = new int[teamCount];
            for (int i = 0; i < teamCount; i++)
                newSlots[i] = teamSlots[i];
            teamSlots = newSlots;
            
            // TODO отправить клиентам новое кол-во команд
        }

        GUILayout.Space(20);
        GUILayout.Label("Наблюдатели:");
        foreach (NetworkConnection conn in observers)
            GUILayout.Label("* " + NetworkManagerCustom.singleton.playerData[conn].Nick);
        if (GUILayout.Button("Перейти"))
            ChangeTeam(-1);

        for (int i = 0; i < teamCount; i++) {
            List<NetworkConnection> team = teams[i];
            GUILayout.Space(20);
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
                
                // TODO отправить клиентам новое кол-во слотов в команде
            }
            
            IEnumerable<string> nicksInTeam = from pair in NetworkManagerCustom.singleton.playerData 
                                              where team.Contains(pair.Key) 
                                              select pair.Value.Nick;

            foreach (string nick in nicksInTeam) 
                GUILayout.Label("* " + nick);

            if (GUILayout.Button("Перейти"))
                ChangeTeam(i);
        }
    }
    
    protected override void OnStartGame() {
        NetworkManagerCustom.singleton.playerConnectedEvent.UnSubcribeEvent(connectEvent);
        NetworkManagerCustom.singleton.playerDisconnectedEvent.UnSubcribeEvent(disconnectEvent);
        PlayerServerData.changeNickEvent.UnSubcribeEvent(changeNickEvent);
    }

    private void ChangeTeam(int to) {
        NetworkConnection conn = NetworkManagerCustom.singleton.FindServerPlayer().Conn;
        int currentTeam = FindTeamOfConnection(conn);
        ChangeTeam(conn, currentTeam, to);
    }

    private void ChangeTeam(NetworkConnection conn, int from, int to) {
        List<NetworkConnection> fromList = from == -1 ? observers : teams[from];
        List<NetworkConnection> toList = to == -1 ? observers : teams[to];
        fromList.Remove(conn);
        toList.Add(conn);
        // TODO синхронизировать с клиентами по playerId
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
