using System;
using UnityEngine;
using UnityEngine.Networking;

public class RailgunClientMessage : GameMessage {

    public RailgunClientMessage() { }

    public RailgunClientMessage(NetworkIdentity identity, int childIndex, Vector2 lineEnd) {
        Writer.Write(identity);
        Writer.Write(childIndex);
        Writer.Write(lineEnd);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        RailgunModule railgunModule = reader.ReadNetworkIdentity().transform.GetChild(reader.ReadInt32()).GetComponentInChildren<RailgunModule>();
        railgunModule.StartCoroutine(railgunModule.RenderLine(reader.ReadVector2()));
    }
}