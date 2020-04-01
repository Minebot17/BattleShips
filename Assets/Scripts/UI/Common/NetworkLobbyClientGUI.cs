using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobbyClientGUI : MonoBehaviour {

	private bool ready = false;

	protected virtual void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);

		if (!ready) {
			GUILayout.Label("Выберите корабль");
			// TODO ship select
		}
		
		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			MessageManager.SetReadyLobbyServerMessage.SendToServer(new StringMessage(ready+""));
		}
	}
}
