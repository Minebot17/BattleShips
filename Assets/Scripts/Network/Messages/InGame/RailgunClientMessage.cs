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
        NetworkIdentity id = reader.ReadNetworkIdentity();
        int childIndex = reader.ReadInt32();
        try {
            RailgunModule railgunModule = childIndex == -1
                                          ? id.gameObject.GetComponent<RailgunModule>()
                                          : id.transform.GetChild(childIndex).GetComponentInChildren<RailgunModule>();
            railgunModule.StartCoroutine(railgunModule.RenderLine(reader.ReadVector2()));
        }
        catch (UnityException e) {
            Debug.Log(e);
        }
    }
}