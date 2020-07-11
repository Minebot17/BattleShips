using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FFAGameMode : IGameMode {
    public int GetEnemyPointerColor(Player from, Player to) {
        return Color.red.ToHex();
    }

    public bool CanDamageModule(ModuleHp hp, DamageInfo source) {
        return true;
    }

    public bool IsRoundOver() {
        return GameObject.FindGameObjectsWithTag("Player").Length <= 1;
    }

    public Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills) {
        if (kills.All(pair => pair.Value == 0))
            kills = kills.ToDictionary(pair => pair.Key, pair => pair.Value + 1);
        
        return kills;
    }
}
