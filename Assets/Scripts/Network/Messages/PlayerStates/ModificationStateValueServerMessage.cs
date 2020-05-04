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
        Player player = Players.GetPlayer(conn);
        int senderId = player.Id;
        int id = reader.ReadInt32();
        if (senderId != id) {
            Debug.LogError("Try modification not own state value");
            Debug.LogError(Environment.StackTrace);
            return;
        }

        string name = reader.ReadString();
        GeneralStateValue stateValue = player.GetStateValue(name);
        
        if (stateValue == null) {
            player.CreateStateWithValue(name);
            stateValue = player.GetStateValue(name);

            if (stateValue == null)
                Debug.LogError(name + " do not exist in any player state");
        }
        
        stateValue.Read(reader, conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}