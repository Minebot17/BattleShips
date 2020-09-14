using System;
using System.Collections.Generic;
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

        Player player = Players.GetPlayer(id);
        CommonState cState = player.GetState<CommonState>();
        GameObject shield = shipObject.gameObject.transform.Find("ShieldRenderer").gameObject;
        if (cState.WithShield.Value) {
            MeshRenderer renderer = shield.GetComponent<MeshRenderer>();
            renderer.enabled = true;
            renderer.material = shieldMaterial;
            List<Vector2> shieldPointsLow = new List<Vector2>();
            List<Vector2> shieldPointsHigh = new List<Vector2>();
            
            for (float x = -20f; x < 20f; x += 0.1f) {
                float D = (float) (Mathf.Pow((float) (shieldVariables[1] * x + shieldVariables[4]), 2) -
                                   4 * shieldVariables[2] * (shieldVariables[0] * x * x + shieldVariables[3] * x - 1));
                if (D < 0)
                    continue;

                shieldPointsLow.Add(new Vector2(x,
                (float) ((-(shieldVariables[1] * x + shieldVariables[4]) - Mathf.Sqrt(D)) / (2 * shieldVariables[2])))
                );
                shieldPointsHigh.Add(new Vector2(x,
                (float) ((-(shieldVariables[1] * x + shieldVariables[4]) + Mathf.Sqrt(D)) / (2 * shieldVariables[2])))
                );
            }

            shieldPointsHigh.Reverse();
            shieldPointsLow.AddRange(shieldPointsHigh);
            shield.GetComponent<PolygonCollider2D>().points = shieldPointsLow.Select(v => v / 20f * 1.3f).ToArray();
            shield.transform.GetChild(0).GetComponent<PolygonCollider2D>().points =
            shieldPointsLow.Select(v => v / 20f * 1.3f).ToArray();
        }
        else if (NetworkManagerCustom.singleton.IsServer)
            shield.transform.GetChild(0).GetComponent<IDeath>().OnDead(null);

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

        if (controller.hasAuthority) {
            CameraFollower.singleton.transform.position = new Vector3(controller.transform.position.x, controller.transform.position.y, CameraFollower.singleton.transform.position.z);
            CameraFollower.singleton.Target = controller.transform;
        }

        if (NetworkManagerCustom.singleton.IsServer)
            shipObject.gameObject.GetComponent<ShipServerController>().enabled = true;
    }
}