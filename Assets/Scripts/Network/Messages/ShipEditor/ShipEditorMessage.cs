using UnityEngine.Networking;

public class ShipEditorMessage : GameMessage {

    public ShipEditorMessage() { }

    public ShipEditorMessage(string json) {
        Writer.Write(json);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        new ShipEditorMessage(NetworkManagerCustom.singleton.playerData[conn].ShipJson).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        ShipEditor.singleton.OpenShip(reader.ReadString());
    }
}