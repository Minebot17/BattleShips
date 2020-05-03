using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class SyncStateValueClientMessage : GameMessage {

    public SyncStateValueClientMessage() { }

    public SyncStateValueClientMessage(int id, string name) {
        Writer.Write(id);
        Writer.Write(name);
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        string name = reader.ReadString();
        PlayerStates states = Players.GetPlayer(id) ?? Players.AddPlayer(id);
        GeneralStateValue stateValue = states.GetStateValue(name);

        if (stateValue == null) {
            states.CreateStateWithValue(name);
            stateValue = states.GetStateValue(name);

            if (stateValue == null)
                Debug.LogError(name + " do not exist in any player state");
        }

        stateValue.Read(reader);
    }

    public override bool WithServersClient() {
        return false;
    }
}