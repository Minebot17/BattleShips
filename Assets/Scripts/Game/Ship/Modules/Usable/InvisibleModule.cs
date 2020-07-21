using UnityEngine;
using UnityEngine.Networking;

public class InvisibleModule : UsableModule {

    [SerializeField] private float invisibleTime;
    
    public override void Use() {
        new InvisibleShipClientMessage(
            Players.GetPlayer(transform.parent.parent.GetComponent<NetworkIdentity>()), invisibleTime
        ).SendToAllClient();
    }
}
