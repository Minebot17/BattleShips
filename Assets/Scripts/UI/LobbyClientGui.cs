using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyClientGui : MonoBehaviour {

	protected bool ready;
	protected string nick;

	protected virtual void Start() {
		Utils.UpdateLocalIPAddress();
		nick = GameSettings.SettingNick.Value.Equals("ip") ? Utils.localIp : GameSettings.SettingNick.Value;
		new SendNickServerMessage(GameSettings.SettingNick.Value).SendToServer();
	}

	protected virtual void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);
		
		GUILayout.Space(10);
		GUILayout.Label("Никнейм:");
		nick = GUILayout.TextField(nick);
		if (GUILayout.Button("OK"))
			new SendNickServerMessage(nick).SendToServer();
		
		RenderInChild();

		GUILayout.Space(10);
		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			new SetReadyLobbyServerMessage(ready).SendToServer();
		}
	}
	
	protected virtual void RenderInChild() {
		
	}
}
