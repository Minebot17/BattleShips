using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipPartsMessage : GameMessage {

    public ShipPartsMessage() { }

    public ShipPartsMessage(NetworkIdentity identity) {
        Writer.Write(identity);
    }
    
    public ShipPartsMessage(NetworkIdentity identity, string json, int id) {
        Writer.Write(identity);
        Writer.Write(json);
        Writer.Write(id);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkIdentity id = reader.ReadNetworkIdentity();
        Player player = Players.GetPlayer(id.clientAuthorityOwner);
        new ShipPartsMessage(id, player.GetState<CommonState>().ShipJson.Value, player.Id).SendToClient(conn);
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity shipObject = reader.ReadNetworkIdentity();
        string json = reader.ReadString();
        int id = reader.ReadInt32();
        Ship ship = Utils.DeserializeShipPartsFromJson(shipObject.gameObject, json);
        ShipController controller = shipObject.gameObject.GetComponent<ShipController>();
        Material shieldMaterial = new Material(controller.shieldShader);
        double[] shieldVariables = Utils.CalculateShieldEllipseVariables(ship.shipModules.Select(m => m.position).ToList());

        char[] abc =  {'A', 'B', 'C', 'D', 'E'};
        for (int i = 0; i < 5; i++)
            shieldMaterial.SetFloat("_EllipseValue" + abc[i], (float) shieldVariables[i]);
            
        MeshRenderer renderer = shipObject.gameObject.transform.Find("ShieldRenderer").GetComponent<MeshRenderer>();
        renderer.enabled = true;
        renderer.material = shieldMaterial;

        Player player = Players.GetPlayer(id);
        CommonState cState = player.GetState<CommonState>();
        if (controller) {
            controller.OnInitializePartsOnClient();

            if (NetworkManagerCustom.singleton.IsServer) {
                ModuleHp aiCoreHp = controller.GetAiCoreModule().GetComponent<ModuleHp>();
                cState.CurrentHealth.Value = aiCoreHp.MaxHealth;
                aiCoreHp.damageEvent.SubcribeEvent(ev =>
                    cState.CurrentHealth.Value = Math.Max(0, cState.CurrentHealth.Value - ev.DamageInfo.Damage)
                );
            }
        }

        cState.ShipIdentity.Value = shipObject;
        Players.BindIdentityToPlayer(player, shipObject);
        if (Players.ClientId != id)
            Utils.SpawnPointer(Players.GetClient(), player);

        if (controller.hasAuthority)
            CameraFollower.singleton.Target = controller.transform;
        
        if (NetworkManagerCustom.singleton.IsServer)
            shipObject.gameObject.GetComponent<ShipServerController>().enabled = true;
    }
}