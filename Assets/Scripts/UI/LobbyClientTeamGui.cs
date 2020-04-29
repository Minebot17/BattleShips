using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyClientTeamGui : LobbyClientGui {
    protected List<int> observers = new List<int>();
    protected List<List<int>> teams = new List<List<int>>();
    protected int teamCount;
    protected int[] teamSlots = new int[0];
    protected Dictionary<int, string> nicks = new Dictionary<int, string>();

    public List<int> Observers { set => observers = value; }
    public List<List<int>> Teams { set => teams = value; }
    public int TeamCount { set => teamCount = value; }
    public int[] TeamSlots { set => teamSlots = value; }
    public Dictionary<int, string> Nicks { set => nicks = value; }

    protected override void Start() {
        base.Start();
        new TeamStateMessage().SendToServer();
    }
    
    protected override void RenderInChild() {
        GUILayout.Space(10);
        GUILayout.Label("Наблюдатели:");
        foreach (int id in observers)
            GUILayout.Label("* " + nicks[id]);
        if (GUILayout.Button("Перейти"))
            ChangeMyTeam(-1);

        for (int i = 0; i < teamCount; i++) {
            List<int> team = teams[i];
            GUILayout.Space(10);
            GUILayout.Label("Команда #" + i);
            GUILayout.Label("Слотов: " + teamSlots[i]);

            IEnumerable<string> nicksInTeam = from pair in nicks
                                              where team.Contains(pair.Key)
                                              select pair.Value;

            foreach (string nick in nicksInTeam) 
                GUILayout.Label("* " + nick);

            if (teams[i].Count < teamSlots[i] && GUILayout.Button("Перейти"))
                ChangeMyTeam(i);
        }
    }

    private void ChangeMyTeam(int to) {
        new ChangeTeamMessage(to).SendToServer();
    }
    
    public void ChangeTeam(int id, int from, int to) {
        List<int> fromList = from == -1 ? observers : teams[from];
        List<int> toList = to == -1 ? observers : teams[to];
        if (!fromList.Contains(id)) {
            from = FindTeamOfConnection(id);
            fromList = from == -1 ? observers : teams[from];
        }

        fromList.Remove(id);
        toList.Add(id);
    }

    public void AddPlayer(int id) {
        observers.Add(id);
        nicks[id] = $"Unknown (id={id})";
    }

    public void RemovePlayer(int id) {
        int team = FindTeamOfConnection(id);
        if (team == -1)
            observers.Remove(id);
        else
            teams[team].Remove(id);
        nicks.Remove(id);
    }

    public void ChangeNick(int id, string nick) {
        nicks[id] = nick;
    }

    public void ChangeTeamCount(int newTeamCount) {
        if (teamCount > newTeamCount) {
            for (int i = newTeamCount; i < teamCount; i++) {
                for (int j = 0; j < teams[i].Count; j++)
                    ChangeTeam(teams[i][j], i, -1);
            }
            teams.RemoveRange(newTeamCount, teamCount - newTeamCount);
        }
        else if (teamCount < newTeamCount) {
            for (int i = 0; i < newTeamCount - teamCount; i++)
                teams.Add(new List<int>());
        }
        
        teamCount = newTeamCount;
    }

    public void ChangeTeamSlots(int team, int newSlots) {
        teamSlots[team] = newSlots;
    }

    private int FindTeamOfConnection(int id) {
        if (observers.Contains(id))
            return -1;
        
        for (int i = 0; i < teams.Count; i++)
            if (teams[i].Contains(id))
                return i;

        return -1;
    }
}
