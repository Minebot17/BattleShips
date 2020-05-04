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
        Player player = Players.GetPlayer(id) ?? Players.AddPlayer(id);
        GeneralStateValue stateValue = player.GetStateValue(name);

        if (stateValue == null) {
            player.CreateStateWithValue(name);
            stateValue = player.GetStateValue(name);

            if (stateValue == null)
                Debug.LogError(name + " do not exist in any player state");
        }

        stateValue.Read(reader);
    }

    public override bool WithServersClient() {
        return false;
    }
}