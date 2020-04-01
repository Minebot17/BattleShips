using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobbyCommon : MonoBehaviour {
	
	private void Awake() {
		MessageManager.Initialize();
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		if (NetworkManagerCustom.singleton.IsServer)
			GameObject.Find("LobbyManager").AddComponent<NetworkLobbyServerHUD>();
		else
			MessageManager.RequestLobbyModeServerMessage.SendToServer(new EmptyMessage());
		Destroy(GetComponent<NetworkLobbyCommon>());
	}
}
