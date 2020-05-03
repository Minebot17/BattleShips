using System;
using UnityEngine;
using UnityEngine.Networking;

public class ModificationStateValueServerMessage : GameMessage {

    public ModificationStateValueServerMessage() { }

    public ModificationStateValueServerMessage(int id, string name) {
        Writer.Write(id);
        Writer.Write(name);
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        PlayerStates states = Players.GetPlayer(conn);
        int senderId = states.Id;
        int id = reader.ReadInt32();
        if (senderId != id) {
            Debug.LogError("Try modification not own state value");
            Debug.LogError(Environment.StackTrace);
            return;
        }

        string name = reader.ReadString();
        GeneralStateValue stateValue = states.GetStateValue(name);
        stateValue.Read(reader);
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}