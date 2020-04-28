using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyClientGUI : MonoBehaviour {

	protected bool ready;
	protected string nick;

	protected virtual void Start() {
		Utils.UpdateLocalIPAddress();
		nick = GameSettings.SettingNick.Value.Equals("ip") ? Utils.localIp : GameSettings.SettingNick.Value;
	}

	protected virtual void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);
		
		GUILayout.Space(20);
		GUILayout.Label("Никнейм:");
		nick = GUILayout.TextField(nick);
		if (GUILayout.Button("OK"))
			MessageManagerOld.SendNickServerMessage.SendToServer(new StringMessage(nick));
		
		RenderInChild();

		GUILayout.Space(20);
		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			new SetReadyLobbyServerMessage(ready).SendToServer();
		}
	}
	
	protected virtual void RenderInChild() {
		
	}
}
