using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobbyClientGUI : MonoBehaviour {

	protected bool ready;

	protected virtual void OnGUI() {
		if (NetworkManagerCustom.singleton.GameInProgress)
			return;
		
		GUILayout.Space(20);
		GUILayout.Label("Вы в лобби у " + NetworkManager.singleton.client.serverIp);

		if (GUILayout.Button(ready ? "Не готов" : "Готов")) {
			ready = !ready;
			MessageManager.SetReadyLobbyServerMessage.SendToServer(new StringMessage(ready+""));
		}
	}
}
