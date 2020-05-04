using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class TeamGameMode : IGameMode {
    
    private List<List<NetworkConnection>> teams;

    public TeamGameMode(List<List<NetworkConnection>> teams) {
        this.teams = teams;
    }

    public int GetEnemyPointerColor(NetworkConnection from, NetworkIdentity to) {
        bool isAlly = teams.Find(c => c.Contains(from)).Contains(to.clientAuthorityOwner);
        return isAlly ? Color.green.ToHex() : Color.red.ToHex();
    }

    public bool CanDamageModule(ModuleHp hp, DamageSource source) {
        if (!(source is PlayerDamageSource))
            return true;
        
        NetworkConnection target = hp.gameObject.transform.parent.parent.gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner;
        List<NetworkConnection> clientCommand = teams.Find(c => c.Contains(((PlayerDamageSource) source).OwnerShip.clientAuthorityOwner));
        bool isAlly = clientCommand.Contains(target);
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

    private List<int> getAliveInTeams() {
        return teams.Select(command => 
            command.Count(conn => Players.GetPlayer(conn).GetState<GameState>().Alive.Value)).ToList();
    }
}
