using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobbyServerGUI : MonoBehaviour {

	private Dictionary<NetworkConnection, bool> readyMap = new Dictionary<NetworkConnection, bool>();
	private bool ready = false;

	public void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		int readyCount = 0;
		foreach (var conn in readyMap.Keys) {
			if (readyMap[conn])
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

		if (!ready) {
			GUILayout.Label("Выберите корабль");
			// TODO ship select
		}
		
		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			SetReady(NetworkManager.singleton.client.connection, ready);
		}

		if (readyCount == connectionsCount && GUILayout.Button("Старт!")) {
			NetworkManagerCustom.singleton.GameInProgress = true;
			NetworkManager.singleton.ServerChangeScene("Start");
		}
	}

	public void SetReady(NetworkConnection conn, bool ready) {
		readyMap[conn] = ready;
	}

	public void RemoveConnection(NetworkConnection conn) {
		readyMap.Remove(conn);
	}
}
