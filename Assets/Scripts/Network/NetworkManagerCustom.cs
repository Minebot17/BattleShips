﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

[AddComponentMenu("NetworkCustom/NetworkManagerCustom")]
public class NetworkManagerCustom : NetworkManager {
	public static NetworkManagerCustom singleton => (NetworkManagerCustom) NetworkManager.singleton;
	public bool IsServer;
	public bool GameInProgress ;
	public List<string> StartArguments; // Информация для установки режима сервера. Задается в классе GUI

	public override void OnServerDisconnect(NetworkConnection conn) {
		if (networkSceneName.Equals("Lobby"))
			GameObject.Find("LobbyManager").GetComponent<NetworkLobbyServerGUI>().RemoveConnection(conn);
	}

	public override void OnServerConnect(NetworkConnection conn) {
		if (GameInProgress) 
			conn.Disconnect();
	}
	
	private void ResetValuesToDefault() {
		IsServer = true;
		GameInProgress = false;
		StartArguments = new List<string>();
	}
	
	public override void OnStopServer() {
		ResetValuesToDefault();
	}

	public void Reset() {
		ResetValuesToDefault();
	}

	public void Start() {
		ResetValuesToDefault();

		if (!Directory.Exists(Application.streamingAssetsPath + "/ships"))
			Directory.CreateDirectory(Application.streamingAssetsPath + "/ships");
	}
}
