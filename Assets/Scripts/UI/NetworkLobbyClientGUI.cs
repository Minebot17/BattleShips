using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobbyClientGUI : MonoBehaviour {

	protected bool ready;
	protected List<string> shipNames;
	public static string selectedShip;

	protected void Start() {
		shipNames = Utils.GetShipNamesList();
		selectedShip = null;
	}

	protected virtual void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);

		if (!ready) {
			GUILayout.Label("Выберите корабль");
			for (int i = 0; i < shipNames.Count; i++)
				if (GUILayout.Button(shipNames[i]))
					selectedShip = shipNames[i];
		}
		
		if (selectedShip != null)
			GUILayout.Label("Вы выбрали: " + selectedShip);
		
		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			MessageManager.SetReadyLobbyServerMessage.SendToServer(new StringMessage(ready+""));
		}
	}
}
