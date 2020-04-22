using System;
using UnityEngine;
using UnityEngine.Networking;

public class SuicideButton : MonoBehaviour {

    private bool isSuicide;
    
    public void OnClick() {
        if (isSuicide)
            return;
        
        MessageManager.SuicideServerMessage.SendToServer(new NetworkIdentityMessage(
            NetworkManagerCustom.singleton.clientShip.GetComponent<NetworkIdentity>()));
    }

}
