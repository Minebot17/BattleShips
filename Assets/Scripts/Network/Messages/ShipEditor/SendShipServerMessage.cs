using System.Collections.Generic;
using UnityEngine.Networking;

public class SendShipServerMessage : GameMessage {

    public SendShipServerMessage() { }

    public SendShipServerMessage(string json, List<int> useModules) {
        Writer.Write(json);
        Writer.Write(new IntegerListMessage(useModules));
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        Player player = Players.GetPlayer(conn);
        player.GetState<CommonState>().ShipJson.Value = reader.ReadString();

        if (player.Id != Players.HostId) {
            InventoryState iState = player.GetState<InventoryState>();
            List<int> useModules = reader.ReadMessage<IntegerListMessage>().Value;
            for (int i = 0; i < useModules.Count; i++)
                if (!ShipEditor.modules[useModules[i]].endlessModule)
                    iState.modulesCount[useModules[i]].Value--;
        }

        if (--NetworkManagerCustom.singleton.lastConnections == 0) 
            NetworkManagerCustom.singleton.ServerChangeScene("Game");
    }
    
    public override void OnClient(NetworkReader reader) {
        
    }
}