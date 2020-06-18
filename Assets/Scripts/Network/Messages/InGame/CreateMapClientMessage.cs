using UnityEngine;
using UnityEngine.Networking;

public class CreateMapClientMessage : GameMessage {

    public CreateMapClientMessage() { }

    public CreateMapClientMessage(string name, Vector2 size) {
        Writer.Write(name);
        Writer.Write(size);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        Map map = new GameObject(reader.ReadString()).AddComponent<Map>();
        map.size = reader.ReadVector2();
    }
}