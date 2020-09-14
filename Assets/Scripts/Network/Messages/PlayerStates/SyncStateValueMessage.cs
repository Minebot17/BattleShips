using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

public class SyncStateValueMessage : GameMessage {

    public SyncStateValueMessage() { }

    public SyncStateValueMessage(int id, string name, bool toConfirm) {
        Writer.Write(id);
        Writer.Write(name);
        Writer.Write(toConfirm);
    }

    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        int id = reader.ReadInt32();
        string name = reader.ReadString();
        GeneralStateValue stateValue = Players.GetPlayer(id).GetStateValue(name);
        stateValue.Confirm();
    }
    
    public override void OnClient(NetworkReader reader) {
        int id = reader.ReadInt32();
        string name = reader.ReadString();
        bool toConfirm = reader.ReadBoolean();
        Player player = Players.GetPlayer(id) ?? Players.AddPlayer(id);
        GeneralStateValue stateValue = player.GetStateValue(name);

        if (stateValue == null) {
            player.CreateStateWithValue(name);
            stateValue = player.GetStateValue(name);

            if (stateValue == null)
                Debug.LogError(name + " do not exist in any player state");
        }

        stateValue.Read(reader, null);
        
        if (toConfirm)
            new SyncStateValueMessage(id, name, true).SendToServer();
    }

    public override bool WithServersClient() {
        return false;
    }
}