using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class SuicideButton : MonoBehaviour {

    private bool isSuicide;
    
    public void OnClick() {
        if (isSuicide)
            return;
        
        MessageManagerOld.SuicideServerMessage.SendToServer(new EmptyMessage());
    }
}
