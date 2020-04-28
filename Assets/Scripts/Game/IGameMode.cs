using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

// server only
public interface IGameMode {
    int GetEnemyPointerColor(NetworkConnection from, NetworkIdentity to);
    bool CanDamageModule(ModuleHp hp, DamageSource source);
    bool IsRoundOver();
    Dictionary<NetworkConnection, int> GetScoreDelta(Dictionary<NetworkConnection, int> kills);
}