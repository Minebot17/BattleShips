﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyServerGui : LobbyClientGui {
	private string[] maps;
	private string roundTime;
	private string roundsCount;

	protected virtual void Start() {
		base.Start();
		GameObject[] goMaps = Resources.LoadAll<GameObject>("Maps/");
		maps = goMaps.Select(go => go.name).ToArray();
		roundTime = global.RoundTime.Value + "";
		roundsCount = global.RoundsCount.Value + "";
	}

	protected override void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;

		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		int readyCount = 0;
		foreach (LobbyState lState in Players.GetStates<LobbyState>()) {
			if (lState.Ready.Value)
				readyCount++;
		}

		int connectionsCount = 0;
		foreach (NetworkConnection conn in NetworkServer.connections) {
			if (conn != null)
				connectionsCount++;
		}
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби. Подключено " + connectionsCount + " игроков");
		GUILayout.Label("Готовы " + readyCount + " из " + connectionsCount);
		GUILayout.Label("Выбранная карта: " + global.CurrentMapName.Value);
		GUILayout.Label("Доступные карты:");
		foreach (string map in maps) {
			if (GUILayout.Button(map))
				global.CurrentMapName.Value = map;
		}

		GUILayout.Space(10);
		GUILayout.Label($"Никнейм: {(validNick ? "" : incorrectNickMessage)}");
		nick = GUILayout.TextField(nick);
		if (GUILayout.Button("OK"))
			cState.Nick.Value = nick;
		
		GUILayout.Space(10);
		GUILayout.Label("Время раунда (секунд): ");
		roundTime = GUILayout.TextField(roundTime);
		if (GUILayout.Button("OK") && int.TryParse(roundTime, out int roundTimeInt))
			global.RoundTime.Value = roundTimeInt;
		
		GUILayout.Space(10);
		GUILayout.Label("Кол-во раундов: ");
		roundsCount = GUILayout.TextField(roundsCount);
		if (GUILayout.Button("OK") && int.TryParse(roundsCount, out int roundsCountInt))
			global.RoundsCount.Value = roundsCountInt;

		GUILayout.Space(10);
		global.WithLootItems.SetWithCheckEquals(GUILayout.Toggle(global.WithLootItems.Value, "С чертежами"));

		RenderInChild();

		LobbyState lStateClient = Players.GetClient().GetState<LobbyState>();
		GUILayout.Space(10);
		if (GUILayout.Button(lStateClient.Ready.Value ? "Не готов" : "Готов"))
			lStateClient.Ready.Value = !lStateClient.Ready.Value;

		if (readyCount == connectionsCount
			&& !NetworkManagerCustom.singleton.GameInProgress
			&& GUILayout.Button("Старт!")) {
			OnStartGame();
			NetworkManagerCustom.singleton.StartGame();
		}
		else if (readyCount == connectionsCount && NetworkManagerCustom.singleton.GameInProgress)
			GUILayout.Label("Загрузка...");
		
		GUILayout.EndScrollView();
	}

	protected virtual void OnStartGame() {
		
	}
}
