using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// server only
public interface IGameMode {
    int GetEnemyPointerColor(Player from, Player to);
    bool CanDamageModule(ModuleHp hp, DamageInfo source);
    bool IsRoundOver();
    void OnSuicide(Player player);
    void OnStartRound();
    Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills);
}
