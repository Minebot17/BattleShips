using System;
using UnityEngine.Networking;

public class RemoveStateMessage : GameMessage {

    public RemoveStateMessage() { }

    public RemoveStateMessage(int id, string stateClass) {
        Writer.Write(id);
        Writer.Write(stateClass);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        int id = reader.ReadInt32();
        string stateClass = reader.ReadString();
        
        Type type = Type.GetType(stateClass);
        Players.GetPlayer(id).RemoveState(type, false);
        
        new RemoveStateMessage(id, stateClass).SendToAllClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        string stateClass = reader.ReadString();
        
        Type type = Type.GetType(stateClass);
        Players.GetPlayer(id).RemoveState(type, false);
    }
}