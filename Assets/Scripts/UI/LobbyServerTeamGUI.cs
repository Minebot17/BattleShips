using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LobbyServerTeamGUI : LobbyServerGui {
    
    protected List<NetworkConnection> notHaveTeam = new List<NetworkConnection>();
    protected List<List<NetworkConnection>> teams = new List<List<NetworkConnection>>();
    protected string teamCountStr = "2";
    protected int teamCount = 2;
    protected string[] teamSlotsStr = { "2", "2" };
    protected int[] teamSlots = { 2, 2 };
    
    protected override void Start() {
        base.Start();
        notHaveTeam.AddRange(nickMap.Keys);
    }
    
    protected override void RenderInChild() {
		GUILayout.Space(20);
        
        teamCountStr = GUILayout.TextField(teamCountStr);
        if (GUILayout.Button("OK")) {
            teamCount = int.Parse(teamCountStr);
            
            int[] newSlots = new int[teamCount];
            for (int i = 0; i < teamCount; i++)
                newSlots[i] = teamSlots[i];
            teamSlots = newSlots;
            
            string[] newSlotsStr = new string[teamCount];
            for (int i = 0; i < teamCount; i++)
                newSlotsStr[i] = teamSlotsStr[i];
            teamSlotsStr = newSlotsStr;
        }

        GUILayout.Space(20);
        GUILayout.Label("Наблюдатели:");
        foreach (KeyValuePair<NetworkConnection, string> pair in nickMap)
            GUILayout.Label("* " + pair.Value);

        for (int i = 0; i < teamCount; i++) {
            GUILayout.Space(20);
            GUILayout.Label("Команда #" + i);
            teamSlotsStr[i] = GUILayout.TextField(teamSlotsStr[i]);
            if (GUILayout.Button("OK")) {
                teamSlots[i] = int.Parse(teamSlotsStr[i]);
                // TODO перекинуть непоместившихся и засинхронить каждое перемещение
            }

            List<NetworkConnection> team = teams[i];
            IEnumerable<string> nicksInTeam = from pair in nickMap 
                                              where team.Contains(pair.Key) 
                                              select pair.Value;

            foreach (string nick in nicksInTeam) 
                GUILayout.Label("* " + nick);

            if (GUILayout.Button("Перейти")) {
                // TODO кинуть пакет на сервер и засинхронить
            }
        }
    }
}
