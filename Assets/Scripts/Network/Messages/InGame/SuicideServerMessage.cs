using UnityEngine.Networking;

public class SuicideServerMessage : GameMessage {

    public SuicideServerMessage() { }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkManagerCustom.singleton.PlayerKill(null, Players.GetPlayer(conn).GetState<GameState>().ShipIdentity.Value);
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}