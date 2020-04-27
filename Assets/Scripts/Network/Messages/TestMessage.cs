using UnityEngine;
using UnityEngine.Networking;

public class TestMessage : GameMessage {

    public TestMessage() { }

    public TestMessage(string message) {
        Writer.Write(message);
    }
    
    public override void OnClient(NetworkReader reader) {
        Debug.Log(reader.ReadString());
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
}