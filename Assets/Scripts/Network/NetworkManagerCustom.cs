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
	public Dictionary<NetworkConnection, string> playerShips = new Dictionary<NetworkConnection, string>();

	public override void OnServerDisconnect(NetworkConnection conn) {
		if (networkSceneName.Equals("Lobby"))
			GameObject.Find("LobbyManager").GetComponent<NetworkLobbyServerGUI>().RemoveConnection(conn);
	}

	public override void OnServerConnect(NetworkConnection conn) {
		if (GameInProgress) 
			conn.Disconnect();
	}

	public override void OnServerSceneChanged(string sceneName) {
		if (sceneName.Equals("Game")) {
			foreach (NetworkConnection conn in playerShips.Keys) {
				if (conn.isReady)
					SpawnClientShip(conn);
				else
					StartCoroutine(WaitForReady(conn));
			}
		}
	}
	
	IEnumerator WaitForReady(NetworkConnection conn) {
		while (!conn.isReady)
			yield return new WaitForSeconds(0.25f);
		
		SpawnClientShip(conn);
	}

	private void SpawnClientShip(NetworkConnection conn) {
		Vector2 spawnPosition = GetSpawnPoint();
		GameObject shipObject = Instantiate(Resources.Load<GameObject>("Prefabs/Ship"));
		shipObject.transform.position = spawnPosition.ToVector3();
		NetworkServer.SpawnWithClientAuthority(shipObject, conn);
		Utils.DeserializeShipPartsFromJson(shipObject, playerShips[conn]);
	}

	private Vector2 GetSpawnPoint() {
		GameObject points = GameObject.Find("SpawnPoints");
		int index = Utils.rnd.Next(points.transform.childCount);
		Vector2 result = new Vector2(points.transform.GetChild(index).position.x, points.transform.GetChild(index).position.x);
		Destroy(points.transform.GetChild(index).gameObject);
		return result;
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
