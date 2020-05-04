using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyServerGui : LobbyClientGui {

	protected override void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
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
		
		GUILayout.Space(10);
		GUILayout.Label($"Никнейм: {(validNick ? "" : incorrectNickMessage)}");
		nick = GUILayout.TextField(nick);

		if (GUILayout.Button("OK"))
			gState.Nick.Value = nick;

		GUILayout.Space(10);
		GUILayout.Label("Кол-во очков до победы");
		NetworkManagerCustom.singleton.scoreForWin = int.Parse(GUILayout.TextField(NetworkManagerCustom.singleton.scoreForWin+""));
		
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
	}
	
	protected override void RenderInChild() {
		
	}

	protected virtual void OnStartGame() {
		
	}
}
