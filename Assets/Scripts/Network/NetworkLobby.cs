﻿using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobby : MonoBehaviour {
	
	private void Awake() {
		GameMessage.Initialize();
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		LobbyClientGui[] found = FindObjectsOfType<LobbyClientGui>();
		GameObject old = found.Length == 0 ? null : found[0].gameObject;
		if (old != null)
			Destroy(old);
		
		new LobbyModeMessage().SendToServer();
		new ClientIndexMessage().SendToServer();

		Destroy(GetComponent<NetworkLobby>());
	}
}
