﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

[AddComponentMenu("NetworkCustom/NetworkManagerCustom")]
public class NetworkManagerCustom : NetworkManager {
	public static NetworkManagerCustom singleton => (NetworkManagerCustom) NetworkManager.singleton;
	public EventHandler<PlayerConnectionEvent> playerConnectedEvent = new EventHandler<PlayerConnectionEvent>();
	public EventHandler<PlayerConnectionEvent> playerDisconnectedEvent = new EventHandler<PlayerConnectionEvent>();
	public static int percentToDeath = 20;

	public bool IsServer;
	public bool GameInProgress;
	public List<string> StartArguments; // Информация для установки режима сервера. Задается в классе GUI
	public Dictionary<NetworkConnection, PlayerServerData> playerData = new Dictionary<NetworkConnection, PlayerServerData>();
	public int clientIndex;
	public int lastConnections;
	public IGameMode gameMode = new FFAGameMode();
	
	public int scoreForWin = 2;
	public GameObject enemyPointerPrefab;
	
	public void Start() {
		ResetValuesToDefault();

		if (!Directory.Exists(Application.streamingAssetsPath + "/ships"))
			Directory.CreateDirectory(Application.streamingAssetsPath + "/ships");
		
		GameSettings.Load();
		Timer.InitializeCreate();
		LanguageManager.Initialize();
		LanguageManager.SetLanguage(x => x.Code.Equals(GameSettings.SettingLanguageCode.Value));
	}

	public override void OnServerConnect(NetworkConnection conn) {
		if (GameInProgress) {
			conn.Disconnect();
			return;
		}

		int id = Utils.rnd.Next();
		playerData.Add(conn, new PlayerServerData() {
			Score = 0,
			Kills = 0,
			ShipJson = Utils.CreateEmptyShip(),
			Alive = true,
			IsShoot = false,
			Nick = conn.address,
			Id = id
		});
		
		playerConnectedEvent.CallListners(new PlayerConnectionEvent(conn));
	}
	
	public override void OnServerDisconnect(NetworkConnection conn) {
		if (networkSceneName.Equals("Lobby"))
			GameObject.Find("LobbyManager").GetComponent<LobbyServerGui>().RemoveConnection(conn);

		playerData.Remove(conn);
		playerDisconnectedEvent.CallListners(new PlayerConnectionEvent(conn));
	}
	
	public override void OnServerSceneChanged(string sceneName) {
		if (sceneName.Equals("ShipEditor"))
			lastConnections = NetworkServer.connections.Count;
		
		if (sceneName.Equals("Game")) {
			foreach (NetworkConnection conn in playerData.Keys) {
				if (conn.isReady)
					SpawnClientShip(conn);
				else
					StartCoroutine(WaitForReady(conn));
			}
		}
	}

	public override void OnClientSceneChanged(NetworkConnection conn) {
		base.OnClientSceneChanged(conn);
		if (networkSceneName.Equals("ShipEditor") && !GameInProgress)
			GameInProgress = true;

		if (networkSceneName.Equals("Lobby") && GameInProgress)
			GameInProgress = false;
	}

	public PlayerServerData FindServerPlayer() {
		return FindPlayerById(clientIndex);
	}

	public PlayerServerData FindPlayerById(int id) {
		return playerData.First(p => p.Value.Id == id).Value;
	}

	public void StartGame() {
		GameInProgress = true;
		NetworkManager.singleton.ServerChangeScene("ShipEditor");
	}

	public void PlayerKill(NetworkIdentity killer, NetworkIdentity prey) {
		if (killer != null) {
			playerData[killer.clientAuthorityOwner].Kills++;
			new KillShipClientMessage(killer, prey).SendToAllClient();
		}

		playerData[prey.clientAuthorityOwner].Alive = false;
		prey.GetComponent<IDeath>().OnDead(null);
		if (gameMode.IsRoundOver())
			Invoke(nameof(RoundOver), 1.9f);
	}
	
	public void RoundOver() {
		ServerChangeScene("Scoreboard");
		Invoke(nameof(ScoreboardOver), Scoreboard.visibleSeconds);
	}

	public void ScoreboardOver() {
		foreach (PlayerServerData data in playerData.Values) {
			data.Score += data.Kills;
			data.Kills = 0;

			if (data.Score == scoreForWin) {
				ServerChangeScene("Lobby");
				DestroyImmediate(GameObject.Find("LobbyManager"));
				GameInProgress = false;
				return;
			}
		}

		ServerChangeScene("ShipEditor");
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
		Utils.DeserializeShipPartsFromJson(shipObject, playerData[conn].ShipJson);
		playerData[conn].ShipIdentity = shipObject.GetComponent<NetworkIdentity>();
		playerData[conn].Alive = true;
	}

	private Vector2 GetSpawnPoint() {
		GameObject points = GameObject.Find("SpawnPoints");
		int index = Utils.rnd.Next(points.transform.childCount);
		Vector2 result = new Vector2(points.transform.GetChild(index).position.x, points.transform.GetChild(index).position.y);
		DestroyImmediate(points.transform.GetChild(index).gameObject);
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

	public class PlayerConnectionEvent : EventBase {
		public NetworkConnection Conn;

		public PlayerConnectionEvent(NetworkConnection conn) : base(singleton, false) {
			Conn = conn;
		}
	}
}
