using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class CommandGameMode : IGameMode {
    
    private List<List<NetworkConnection>> commands = new List<List<NetworkConnection>>();

    public int GetEnemyPointerColor(NetworkConnection from, NetworkIdentity to) {
        bool isAlly = commands.Find(c => c.Contains(from)).Contains(to.clientAuthorityOwner);
        return isAlly ? Color.green.ToHex() : Color.red.ToHex();
    }

    public bool CanDamageModule(ModuleHp hp, DamageSource source) {
        if (!(source is PlayerDamageSource))
            return true;
        
        NetworkConnection target = hp.gameObject.transform.parent.parent.gameObject.GetComponent<NetworkIdentity>().clientAuthorityOwner;
        List<NetworkConnection> clientCommand = commands.Find(c => c.Contains(((PlayerDamageSource) source).OwnerShip.clientAuthorityOwner));
        bool isAlly = clientCommand.Contains(target);
        return !isAlly;
    }

    public bool IsRoundOver() {
        List<int> playersAlive = commands.Select(command => 
            command.Count(id => NetworkManagerCustom.singleton.playerData[id].shipIdentity)).ToList();
        
        return playersAlive.Count(c => c == 0) == playersAlive.Count - 1;
    }

    public Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills) {
        List<int> playersAlive = commands.Select(command => command.Count(id => NetworkManagerCustom.singleton.playerData[id].alive)).ToList();
        int winnersIndex = playersAlive.FindIndex(c => c != 0);
        List<NetworkConnection> winners = commands[winnersIndex];
        
        Dictionary<NetworkConnection, int> scoreDelta = new Dictionary<NetworkConnection, int>();
        foreach (NetworkConnection conn in NetworkManagerCustom.singleton.playerData.Keys) 
            scoreDelta.Add(conn, winners.Contains(conn) ? 1 : 0);

        return scoreDelta;
    }
}
