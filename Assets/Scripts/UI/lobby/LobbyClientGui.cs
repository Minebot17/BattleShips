﻿using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyClientGui : MonoBehaviour {

	protected static Regex nickTemplate = new Regex("([a-zA-Z0-9.]){1,16}");
	protected static string incorrectNickMessage = "Incorrect Nick";

	protected string nick;
	protected bool validNick = true;

	protected GameState gState;
	protected LobbyState lState;

	protected virtual void Start() {
		gState = Players.GetClient().GetState<GameState>();
		lState = Players.GetClient().GetState<LobbyState>();

		gState.Nick.Value = GameSettings.SettingNick.Value.Equals("ip") ? ("Player " + gState.GetParent().Id) : GameSettings.SettingNick.Value;
		nick = gState.Nick.Value;
		gState.Nick.onChangeValueEvent.SubcribeEvent(e => {
			if (!nickTemplate.IsMatch(e.NewValue)) {
				validNick = false;
				e.IsCancel = true;
			}
			else {
				validNick = true;
				GameSettings.SettingNick.Value = nick;
				GameSettings.SettingNick.Save();
			}
		});
	}

	protected virtual void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);
		
		GUILayout.Space(10);
		GUILayout.Label($"Никнейм: {(validNick ? "" : incorrectNickMessage)}");
		nick = GUILayout.TextField(nick);

		if (GUILayout.Button("OK"))
			gState.Nick.Value = nick;

		RenderInChild();

		GUILayout.Space(10);
		if (GUILayout.Button(lState.Ready.Value ? "Не готов" : "Готов"))
			lState.Ready.Value = !lState.Ready.Value;
	}

	protected virtual void RenderInChild() {
		GUILayout.Space(10);
		GUILayout.Label("Игроки:");
		foreach (GameState state in Players.GetStates<GameState>())
			GUILayout.Label(state.Nick.Value + " (" + (state.GetParent().GetState<LobbyState>().Ready.Value ? "Готов" : "Не готов") + ")");
	}
}