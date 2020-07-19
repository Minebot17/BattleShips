using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FFAGameMode : IGameMode {

    private int suicides;
    private NetworkConnection lastPlayer;
    
    public int GetEnemyPointerColor(Player from, Player to) {
        return Color.red.ToHex();
    }

    public bool CanDamageModule(ModuleHp hp, DamageInfo source) {
        return true;
    }

    public bool IsRoundOver() {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        bool isOver = players.Length <= 1;
        if (isOver && players.Length != 0)
            lastPlayer = players[0].GetComponent<NetworkIdentity>().clientAuthorityOwner;
        return isOver;
    }

    public void OnSuicide(Player player) {
        suicides++;
    }

    public void OnStartRound() {
        suicides = 0;
        lastPlayer = null;
    }

    public Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills) {
        if (lastPlayer != null && suicides != 0)
            kills[lastPlayer] += suicides;

        if (kills.All(pair => pair.Value == 0))
            kills = kills.ToDictionary(pair => pair.Key, pair => pair.Value + 1);
        
        return kills;
    }
}
