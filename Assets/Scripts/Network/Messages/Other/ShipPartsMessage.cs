using UnityEngine.Networking;

public class ShipPartsMessage : GameMessage {

    public ShipPartsMessage() { }

    public ShipPartsMessage(NetworkIdentity identity) {
        Writer.Write(identity);
    }
    
    public ShipPartsMessage(NetworkIdentity identity, string json) {
        Writer.Write(identity);
        Writer.Write(json);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkIdentity id = reader.ReadNetworkIdentity();
        new ShipPartsMessage(id, NetworkManagerCustom.singleton.playerData[id.clientAuthorityOwner].ShipJson).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity shipObject = reader.ReadNetworkIdentity();
        string json = reader.ReadString();
        Utils.DeserializeShipPartsFromJson(shipObject.gameObject, json);

        ShipController controller = shipObject.gameObject.GetComponent<ShipController>();
        if (controller) 
            controller.OnInitializePartsOnClient();
    }
}