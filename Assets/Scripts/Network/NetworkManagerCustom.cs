﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

[AddComponentMenu("NetworkCustom/NetworkManagerCustom")]
public class NetworkManagerCustom : NetworkManager {
	public static NetworkManagerCustom singleton => (NetworkManagerCustom) NetworkManager.singleton;
	public static int percentToDeath = 20;
	public bool IsServer;
	public bool GameInProgress;
	public List<string> StartArguments; // Информация для установки режима сервера. Задается в классе GUI
	public Dictionary<NetworkConnection, PlayerServerData> playerData = new Dictionary<NetworkConnection, PlayerServerData>();
	public GameObject clientShip;
	public int lastConnections;
	public IGameMode gameMode = new FFAGameMode();
	
	public int scoreForWin = 2;
	public GameObject enemyPointerPrefab;
	
	// TODO сделать эвенты на сервере при подключении и отключении игроков
	public override void OnServerDisconnect(NetworkConnection conn) {
		if (networkSceneName.Equals("Lobby"))
			GameObject.Find("LobbyManager").GetComponent<LobbyServerGui>().RemoveConnection(conn);
	}

	public override void OnServerConnect(NetworkConnection conn) {
		if (GameInProgress) 
			conn.Disconnect();
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

	public void StartGame() {
		GameInProgress = true;
		LobbyServerGui serverGui = GameObject.Find("LobbyManager").GetComponent<LobbyServerGui>();
		foreach (NetworkConnection conn in NetworkServer.connections) {
				playerData[conn] = new PlayerServerData() {
				score = 0,
				kills = 0,
				shipJson = Utils.CreateEmptyShip(),
				alive = true,
				isShoot = false,
				nick = serverGui.nickMap[conn]
			};
		}

		NetworkManager.singleton.ServerChangeScene("ShipEditor");
	}

	public void PlayerKill(NetworkIdentity killer, NetworkIdentity prey) {
		if (killer != null) {
			playerData[killer.clientAuthorityOwner].kills++;
			MessageManager.KillShipClientMessage.SendToAllClients(new MessagesMessage(new MessageBase[] {
				new NetworkIdentityMessage(killer),
				new NetworkIdentityMessage(prey)
			}));
		}

		playerData[prey.clientAuthorityOwner].alive = false;
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
			data.score += data.kills;
			data.kills = 0;

			if (data.score == scoreForWin) {
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
		Utils.DeserializeShipPartsFromJson(shipObject, playerData[conn].shipJson);
		playerData[conn].shipIdentity = shipObject.GetComponent<NetworkIdentity>();
		playerData[conn].alive = true;
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

	public void Start() {
		ResetValuesToDefault();

		if (!Directory.Exists(Application.streamingAssetsPath + "/ships"))
			Directory.CreateDirectory(Application.streamingAssetsPath + "/ships");
		
		GameSettings.Load();
		Timer.InitializeCreate();
		LanguageManager.Initialize();
		LanguageManager.SetLanguage(x => x.Code.Equals(GameSettings.SettingLanguageCode.Value));
	}
}
