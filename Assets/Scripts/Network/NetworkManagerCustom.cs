﻿﻿using System;
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
	public static int percentToDeath = 20;
	public static GameObject lobbyManager;

	public bool IsServer;
	public bool GameInProgress;
	public List<string> StartArguments; // Информация для установки режима сервера. Задается в классе GUI
	public IGameMode gameMode = new FFAGameMode();
	public int lastConnections;
	
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
		Players.Initialize();
	}

	public override void OnServerConnect(NetworkConnection conn) {
		if (GameInProgress) {
			conn.Disconnect();
			return;
		}

		Players.AddPlayer(conn);
	}
	
	public override void OnServerDisconnect(NetworkConnection conn) {
		Players.RemovePlayer(conn);
	}
	
	public override void OnServerSceneChanged(string sceneName) {
		if (sceneName.Equals("ShipEditor"))
			lastConnections = Players.All.Count;

		if (sceneName.Equals("Game")) {
			foreach (NetworkConnection conn in Players.Conns) {
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

		if (networkSceneName.Equals("Lobby") && GameInProgress) {
			GameInProgress = false;
		}
	}
	
	public void StartGame() {
		GameInProgress = true;
		NetworkManager.singleton.ServerChangeScene("ShipEditor");
	}

	public void PlayerKill(NetworkIdentity killer, NetworkIdentity prey) {
		if (killer != null) {
			Players.GetPlayer(killer.clientAuthorityOwner).GetState<GameState>().Kills.Value++;
			new KillShipClientMessage(killer, prey).SendToAllClient();
		}

		Players.GetPlayer(prey.clientAuthorityOwner).GetState<GameState>().Alive.Value = false;
		prey.GetComponent<IDeath>().OnDead(null);
		if (gameMode.IsRoundOver())
			Invoke(nameof(RoundOver), 1.9f);
	}
	
	public void RoundOver() {
		ServerChangeScene("Scoreboard");
		Invoke(nameof(ScoreboardOver), Scoreboard.visibleSeconds);
	}

	public void ScoreboardOver() {
		bool gameOver = false;
		foreach (GameState gState in Players.GetStates<GameState>()) {
			gState.Score.Value += gState.Kills.Value;
			gState.Kills.Value = 0;

			if (gState.Score.Value >= scoreForWin) {
				gameOver = true;
				break;
			}
		}

		if (gameOver) {
			foreach (Player player in Players.All)
				player.ResetStates();

			DestroyImmediate(lobbyManager);
			ServerChangeScene("Lobby");
			GameInProgress = false;
			return;
		}

		ServerChangeScene("ShipEditor");
	}

	IEnumerator WaitForReady(NetworkConnection conn) {
		while (!conn.isReady)
			yield return new WaitForSeconds(0.25f);
		
		SpawnClientShip(conn);
	}

	void SpawnClientShip(NetworkConnection conn) {
		Vector2 spawnPosition = GetSpawnPoint();
		GameObject shipObject = Instantiate(Resources.Load<GameObject>("Prefabs/Ship"));
		shipObject.transform.position = spawnPosition.ToVector3();
		NetworkServer.SpawnWithClientAuthority(shipObject, conn);

		GameState gState = Players.GetPlayer(conn).GetState<GameState>();
		gState.ShipIdentity.Value = shipObject.GetComponent<NetworkIdentity>();
		gState.Alive.Value = true;
	}

	Vector2 GetSpawnPoint() {
		GameObject points = GameObject.Find("SpawnPoints");
		int index = Utils.rnd.Next(points.transform.childCount);
		Vector2 result = new Vector2(points.transform.GetChild(index).position.x, points.transform.GetChild(index).position.y);
		DestroyImmediate(points.transform.GetChild(index).gameObject);
		return result;
	}

	void ResetValuesToDefault() {
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
}
