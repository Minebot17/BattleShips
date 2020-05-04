using UnityEngine.Networking;

public class ShipPartsMessage : GameMessage {

    public ShipPartsMessage() { }

    public ShipPartsMessage(NetworkIdentity identity) {
        Writer.Write(identity);
    }
    
    public ShipPartsMessage(NetworkIdentity identity, string json, int id) {
        Writer.Write(identity);
        Writer.Write(json);
        Writer.Write(id);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkIdentity id = reader.ReadNetworkIdentity();
        Player player = Players.GetPlayer(id.clientAuthorityOwner);
        new ShipPartsMessage(id, player.GetState<GameState>().ShipJson.Value, player.Id).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity shipObject = reader.ReadNetworkIdentity();
        string json = reader.ReadString();
        int id = reader.ReadInt32();
        Utils.DeserializeShipPartsFromJson(shipObject.gameObject, json);

        ShipController controller = shipObject.gameObject.GetComponent<ShipController>();
        if (controller) 
            controller.OnInitializePartsOnClient();

        Player player = Players.GetPlayer(id);
        player.GetState<GameState>().ShipIdentity.Value = shipObject;
        if (Players.GetClient().Id != id)
            Utils.SpawnPointer(Players.GetClient(), player);
    }
}