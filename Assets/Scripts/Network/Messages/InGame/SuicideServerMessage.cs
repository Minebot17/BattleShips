using UnityEngine.Networking;

public class SuicideServerMessage : GameMessage {

    public SuicideServerMessage() { }

    public SuicideServerMessage(NetworkIdentity ship) {
        Writer.Write(ship);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        new SuicideServerMessage(Players.GetPlayer(conn).GetState<CommonState>().ShipIdentity.Value).SendToAllClient();
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity ship = reader.ReadNetworkIdentity();
        GameCoroutines.Singleton.StartCoroutine(GameCoroutines.SuicideCoroutine(ship));
    }
}