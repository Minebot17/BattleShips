using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class InvisibleShipClientMessage : GameMessage {

    public InvisibleShipClientMessage() { }

    public InvisibleShipClientMessage(Player player, float invisibleTime) {
        Writer.Write(player);
        Writer.Write(invisibleTime);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        Player clientPlayer = Players.GetClient();
        Player invisiblePlayer = reader.ReadPlayer();
        NetworkIdentity invisiblePlayerShip = invisiblePlayer.GetShip();
        float invisibleTime = reader.ReadSingle();
        SpriteRenderer[] allSpriteRenderers = invisiblePlayerShip.GetComponentsInChildren<SpriteRenderer>();
        float newAlpha = clientPlayer == invisiblePlayer ? 0.25f : 0f;
        
        foreach (SpriteRenderer spriteRenderer in allSpriteRenderers) {
            Color newColor = spriteRenderer.color;
            newColor.a = newAlpha;
            spriteRenderer.color = newColor;
        }

        if (clientPlayer != invisiblePlayer) {
            Transform shield = invisiblePlayerShip.transform.Find("ShieldRenderer");
            if (shield)
                shield.GetComponent<MeshRenderer>().enabled = false;
        }

        invisiblePlayerShip.GetComponent<ShipController>().StartCoroutine(DisableInvisibleCoroutine(invisiblePlayer, invisiblePlayerShip, allSpriteRenderers, invisibleTime));
    }

    private static IEnumerator DisableInvisibleCoroutine(Player player, NetworkIdentity invisiblePlayerShip, SpriteRenderer[] allSpriteRenderers, float time) {
        yield return new WaitForSeconds(time);
        
        if (!invisiblePlayerShip)
            yield break;
        
        Transform shield = invisiblePlayerShip.transform.Find("ShieldRenderer");
        if (shield)
            shield.GetComponent<MeshRenderer>().enabled = true;
        
        foreach (SpriteRenderer spriteRenderer in allSpriteRenderers) {
            Color newColor = spriteRenderer.color;
            newColor.a = 1f;
            spriteRenderer.color = newColor;
        }

        if (NetworkManagerCustom.singleton.IsServer)
            player.GetState<CommonState>().IsInvisible.Value = false;
    }
}