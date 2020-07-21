using UnityEngine;
using UnityEngine.Networking;

public class InvisibleModule : UsableModule {

    public override void Use() {
        Player player = Players.GetPlayer(transform.parent.parent.GetComponent<NetworkIdentity>());
        player.GetState<CommonState>().IsInvisible.Value = true;
        float current = 9f/(1f + Mathf.Exp(5f/3f - GetInstalledCount())) - 1f;
        new InvisibleShipClientMessage(player, current).SendToAllClient();
    }
}
