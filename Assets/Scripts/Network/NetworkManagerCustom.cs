﻿﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.SceneManagement;

[AddComponentMenu("NetworkCustom/NetworkManagerCustom")]
public class NetworkManagerCustom : NetworkManager {
	public static NetworkManagerCustom singleton => (NetworkManagerCustom) NetworkManager.singleton;
	public static GameObject lobbyManager;

	public bool IsServer;
	public bool GameInProgress;
	public List<string> StartArguments; // Информация для установки режима сервера. Задается в классе GUI
	public IGameMode gameMode = new FFAGameMode();
	public int lastConnections;
	public GameObject enemyPointerPrefab;

	public void Start() {
		ResetValuesToDefault();
		ShipEditor.Initialize();
		GameSettings.Load();
		Timer.InitializeCreate();
		LanguageManager.Initialize();
		LanguageManager.SetLanguage(x => x.Code.Equals(GameSettings.SettingLanguageCode.Value));
		Players.Initialize();
		Map.Initialize();
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
		if (sceneName.Equals("ShipEditor")) {
			lastConnections = Players.All.Count;

			List<Vector2> spawnPoints = Resources.Load<GameObject>("Maps/" + Players.GetGlobal().CurrentMapName.Value)
											.GetComponent<Map>().SpawnPoints;

			ValueObservable<int> confirmCount = new ValueObservable<int>(0);
			confirmCount.WhenEqual(Players.All.Count, () => {
				Map.SpawnMap(Players.GetGlobal().CurrentMapName.Value, true);
			});
			
			foreach (CommonState cState in Players.GetStates<CommonState>()) {
				(int, Vector2) random = spawnPoints.GetRandom();
				spawnPoints.RemoveAt(random.Item1);
				cState.SpawnPoint.onAllSynced += () => confirmCount.Value++;
				cState.SpawnPoint.Value = random.Item2;
			}
		}

		if (sceneName.Equals("Game")) {
			gameMode.OnStartRound();
			Map.SpawnMap(Players.GetGlobal().CurrentMapName.Value);
			//SceneManager.MoveGameObjectToScene(Map.singleton.gameObject, SceneManager.GetActiveScene());
			//SceneManager.MoveGameObjectToScene(CameraFollower.singleton.gameObject, SceneManager.GetActiveScene());

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
		if (killer != null && killer != prey)
			Players.GetPlayer(killer.clientAuthorityOwner).GetState<CommonState>().Kills.Value++;
		else
			gameMode.OnSuicide(Players.GetPlayer(prey));
		
		new KillShipClientMessage(prey).SendToAllClient();

		Players.GetPlayer(prey.clientAuthorityOwner).GetState<CommonState>().Alive.Value = false;
		prey.GetComponent<IDeath>().OnDead(null);
		if (gameMode.IsRoundOver())
			Invoke(nameof(RoundOver), 1.9f);
	}
	
	public void RoundOver() {
		ServerChangeScene("Scoreboard");
		Invoke(nameof(ScoreboardOver), Scoreboard.visibleSeconds);
	}

	public void ScoreboardOver() {
		foreach (GameObject scoreKey in Scoreboard.singleton.deltaScore.Keys) {
			int id = int.Parse(scoreKey.name.Split(' ')[1]);
			CommonState cState = Players.GetPlayer(id).GetState<CommonState>();
			cState.Score.Value += Scoreboard.singleton.deltaScore[scoreKey];
			cState.Kills.Value = 0;
		}

		GlobalState gState = Players.GetGlobal();
		foreach (CommonState cState in Players.GetStates<CommonState>()) {
			if (cState.Score.Value >= gState.RoundsCount.Value) {
				foreach (Player player in Players.All)
					player.ResetStates();

				DestroyImmediate(lobbyManager);
				ServerChangeScene("Lobby");
				GameInProgress = false;
				return;
			}
		}

		ServerChangeScene("ShipEditor");
	}

	private IEnumerator WaitForReady(NetworkConnection conn) {
		while (!conn.isReady)
			yield return new WaitForSeconds(0.25f);
		
		SpawnClientShip(conn);
	}

	private void SpawnClientShip(NetworkConnection conn) {
		Vector2 spawnPosition = Players.GetPlayer(conn).GetState<CommonState>().SpawnPoint.Value;
		GameObject shipObject = Instantiate(Resources.Load<GameObject>("Prefabs/Ship"));
		shipObject.transform.position = spawnPosition.ToVector3();
		NetworkServer.SpawnWithClientAuthority(shipObject, conn);

		CommonState gState = Players.GetPlayer(conn).GetState<CommonState>();
		gState.ShipIdentity.Value = shipObject.GetComponent<NetworkIdentity>();
		gState.Alive.Value = true;
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
}
