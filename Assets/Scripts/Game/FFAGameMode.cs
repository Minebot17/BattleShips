using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class FFAGameMode : IGameMode {
    public int GetEnemyPointerColor(Player from, Player to) {
        return Color.red.ToHex();
    }

    public bool CanDamageModule(ModuleHp hp, DamageSource source) {
        return true;
    }

    public bool IsRoundOver() {
        return GameObject.FindGameObjectsWithTag("Player").Length <= 1;
    }

    public Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills) {
        return kills;
    }
}
