using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendShipServerMessage : GameMessage {

    public SendShipServerMessage() { }

    public SendShipServerMessage(string json, List<int> useModules) {
        Writer.Write(json);
        Writer.Write(new IntegerListMessage(useModules));
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        Player player = Players.GetPlayer(conn);
        CommonState cState = player.GetState<CommonState>();
        cState.ShipJson.Value = reader.ReadString();
        
        List<int> useModules = reader.ReadMessage<IntegerListMessage>().Value;
        int delta = Players.GetGlobal().BuildPointsPerRound.Value - useModules.Count;
        if (delta != 0)
            cState.AdditionalBuildPoints.Value += delta;

        if (player.Id != Players.HostId) {
            InventoryState iState = player.GetState<InventoryState>();
            foreach (int moduleIndex in useModules)
                if (!ShipEditor.modules[moduleIndex].endlessModule)
                    iState.modulesCount[moduleIndex].Value--;
        }

        if (--NetworkManagerCustom.singleton.lastConnections == 0) 
            NetworkManagerCustom.singleton.ServerChangeScene("Game");
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}