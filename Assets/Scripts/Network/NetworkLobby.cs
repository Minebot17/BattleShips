using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobby : MonoBehaviour {
	
	private void Awake() {
		MessageManager.Initialize();
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		LobbyClientGUI[] found = FindObjectsOfType<LobbyClientGUI>();
		GameObject old = found.Length == 0 ? null : found[0].gameObject;
		if (old != null)
			Destroy(old);

		// TODO пизда расширяемости игровых режимов из-за этого свича (на клиенте в MessageManager тоже свич есть)
		if (NetworkManagerCustom.singleton.IsServer) {
			string mode = NetworkManagerCustom.singleton.StartArguments.Find(s => s.StartsWith("gamemode:")).Split(':')[1];
			GameObject lobbyManager = GameObject.Find("LobbyManager");
		
			switch (mode) {
				case "ffa":
					lobbyManager.AddComponent<LobbyServerGui>();
					break;
				case "commands":
					lobbyManager.AddComponent<LobbyServerTeamGUI>();
					break;
			}
		}
		else {
			MessageManager.RequestLobbyModeServerMessage.SendToServer(new EmptyMessage());
		}

		Destroy(GetComponent<NetworkLobby>());
	}
}
