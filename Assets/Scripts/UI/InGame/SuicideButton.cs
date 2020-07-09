using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class SuicideButton : MonoBehaviour {

    public void OnClick() {
        new SuicideServerMessage().SendToServer();
        Destroy(gameObject);
    }
}
