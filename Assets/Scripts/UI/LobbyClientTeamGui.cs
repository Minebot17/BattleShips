using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyClientTeamGui : LobbyClientGui {
    protected List<int> observers = new List<int>();
    protected List<List<int>> teams = new List<List<int>>();
    protected int teamCount = 2;
    protected int[] teamSlots = { 2, 2 };
    protected Dictionary<int, string> nicks;

    protected override void Start() {
        base.Start();
        // TODO запрос на ники и состояние
    }
    
    protected override void RenderInChild() {
        GUILayout.Space(20);
        GUILayout.Label("Наблюдатели:");
        foreach (int id in observers)
            GUILayout.Label("* " + nicks[id]);
        if (GUILayout.Button("Перейти"))
            ChangeTeam(-1);

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

            if (GUILayout.Button("Перейти"))
                ChangeTeam(i);
        }
    }

    private void ChangeTeam(int to) {
        // TODO кинуть пакет на замену команды
    }
}
