using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class LobbyClientGui : MonoBehaviour {

	protected static Regex nickTemplate = new Regex("([a-zA-Z0-9.]){1,16}");
	protected static string incorrectNickMessage = "Incorrect Nick";

	protected string nick;
	protected bool validNick = true;

	protected CommonState cState;
	protected LobbyState lState;
	protected GlobalState global;
	protected Vector2 scrollPosition;

	protected virtual void Start() {
		cState = Players.GetClient().GetState<CommonState>();
		lState = Players.GetClient().GetState<LobbyState>();
		global = Players.GetGlobal();

		cState.Nick.Value = GameSettings.SettingNick.Value.Equals("ip") ? ("Player " + cState.GetParent().Id) : GameSettings.SettingNick.Value;
		nick = cState.Nick.Value;
		cState.Nick.onChangeValueEvent.SubcribeEvent(e => {
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
		
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);
		GUILayout.Label("Выбранная карта: " + global.CurrentMapName.Value);
		
		GUILayout.Space(10);
		GUILayout.Label($"Никнейм: {(validNick ? "" : incorrectNickMessage)}");
		nick = GUILayout.TextField(nick);
		if (GUILayout.Button("OK"))
			cState.Nick.Value = nick;
		
		GUILayout.Space(10);
		GUILayout.Label("Время раунда: " + global.RoundTime.Value + " секунд");
		
		GUILayout.Space(10);
		GUILayout.Label("Кол-во раундов: " + global.RoundsCount.Value);

		RenderInChild();

		GUILayout.Space(10);
		if (GUILayout.Button(lState.Ready.Value ? "Не готов" : "Готов"))
			lState.Ready.Value = !lState.Ready.Value;
		
		GUILayout.EndScrollView();
	}

	protected virtual void RenderInChild() {
		GUILayout.Space(10);
		GUILayout.Label("Игроки:");
		foreach (CommonState state in Players.GetStates<CommonState>())
			GUILayout.Label(state.Nick.Value + " (" + (state.GetParent().GetState<LobbyState>().Ready.Value ? "Готов" : "Не готов") + ")");
	}
}
