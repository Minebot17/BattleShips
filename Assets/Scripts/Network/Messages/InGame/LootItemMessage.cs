using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class LootItemMessage : GameMessage {

    public LootItemMessage() { }

    public LootItemMessage(NetworkIdentity identity) {
        Writer.Write(identity);
    }
    
    public LootItemMessage(NetworkIdentity identity, string editorModuleName) {
        Writer.Write(identity);
        Writer.Write(editorModuleName);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        new LootItemMessage(identity, identity.gameObject.name.Split('_')[1]).SendToClient(conn); 
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity identity = reader.ReadNetworkIdentity();
        string editorModuleName = reader.ReadString();
        identity.name = "LootItem_" + editorModuleName;
        identity.GetComponent<SpriteRenderer>().sprite = 
        ShipEditor.modules
               .First(m => m.name.Equals(editorModuleName)).prefab
               .GetComponent<SpriteRenderer>().sprite;
    }
}