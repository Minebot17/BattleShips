using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamGameMode : IGameMode {
    List<List<NetworkConnection>> teams;

    public TeamGameMode(List<List<NetworkConnection>> teams) {
        this.teams = teams;
        
        if (teams == null)
            return;
        
        for(int x = 0; x < teams.Count; x++)
            for (int y = 0; y < teams[x].Count; y++) {
                Player player = Players.GetPlayer(teams[x][y]);
                player.GetState<TeamState>().TeamIndex.Value = x;
            }
    }

    public int GetEnemyPointerColor(Player from, Player to) {
        bool isAlly = from.GetState<TeamState>().TeamIndex.Value == to.GetState<TeamState>().TeamIndex.Value;
        return isAlly ? Color.green.ToHex() : Color.red.ToHex();
    }

    public bool CanDamageModule(ModuleHp hp, DamageInfo source) {
        if (!NetworkManagerCustom.singleton.IsServer || !(source.OwnerShip == null))
            return true;
        
        NetworkConnection target = hp.gameObject.transform.parent.parent.gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner;
        NetworkConnection owner = source.OwnerShip.clientAuthorityOwner;
        bool isAlly = Players.GetPlayer(target).GetState<TeamState>().TeamIndex.Value == Players.GetPlayer(owner).GetState<TeamState>().TeamIndex.Value;
        return !isAlly;
    }

    public bool IsRoundOver() {
        List<int> playersAlive = getAliveInTeams();
        return playersAlive.Count(c => c == 0) == playersAlive.Count - 1;
    }

    public Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills) {
        int winnersIndex = getAliveInTeams().FindIndex(c => c != 0);
        List<NetworkConnection> winners = teams[winnersIndex];
        
        Dictionary<NetworkConnection, int> scoreDelta = new Dictionary<NetworkConnection, int>();
        foreach (NetworkConnection conn in Players.Conns) 
            scoreDelta.Add(conn, winners.Contains(conn) ? 1 : 0);

        return scoreDelta;
    }

    List<int> getAliveInTeams() {
        return teams.Select(command => 
            command.Count(conn => Players.GetPlayer(conn).GetState<CommonState>().Alive.Value)).ToList();
    }
}
