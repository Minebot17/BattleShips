using UnityEngine.Networking;

public class SuicideServerMessage : GameMessage {

    public SuicideServerMessage() { }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkManagerCustom.singleton.PlayerKill(null, NetworkManagerCustom.singleton.playerData[conn].ShipIdentity);
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}