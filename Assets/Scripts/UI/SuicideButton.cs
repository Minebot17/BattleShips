using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class SuicideButton : MonoBehaviour {
    bool isSuicide;
    
    public void OnClick() {
        if (isSuicide)
            return;
        
        new SuicideServerMessage().SendToServer();
    }
}
