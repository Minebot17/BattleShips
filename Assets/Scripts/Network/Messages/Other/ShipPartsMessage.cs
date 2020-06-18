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
        new ShipPartsMessage(id, player.GetState<CommonState>().ShipJson.Value, player.Id).SendToClient(conn);
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
        player.GetState<CommonState>().ShipIdentity.Value = shipObject;
        if (Players.ClientId != id)
            Utils.SpawnPointer(Players.GetClient(), player);

        if (NetworkManagerCustom.singleton.IsServer)
            shipObject.gameObject.GetComponent<ShipServerController>().enabled = true;
    }
}