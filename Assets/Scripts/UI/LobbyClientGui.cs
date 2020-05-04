using System;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyClientGui : MonoBehaviour {

	protected bool ready;
	protected string nick;
	protected bool validNick = true;
	protected string incorrectNickMessage = "Incorrect Nick";

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
		GUILayout.Label($"Никнейм: {(validNick ? "" : incorrectNickMessage)}");
		nick = GUILayout.TextField(nick);

		validNick = SendNickServerMessage.nickTemplate.IsMatch(nick);
		if (GUILayout.Button("OK") && validNick)
			new SendNickServerMessage(nick).SendToServer();

		RenderInChild();

		GUILayout.Space(10);
		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			new SetReadyLobbyServerMessage(ready).SendToServer();
		}
	}

	public void SetNick(bool valid)
	{
		if (valid)
		{
			GameSettings.SettingNick.Value = nick;
			GameSettings.SettingNick.Save();
		}
		else
		{
			validNick = false;
		}
	}
	
	protected virtual void RenderInChild() {
		
	}
}
