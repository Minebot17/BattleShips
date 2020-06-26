using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobby : MonoBehaviour {
	private void Awake() {
		NetworkManagerCustom.lobbyManager = GameObject.Find("LobbyManager");
		GameMessage.Initialize();
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		LobbyClientGui[] found = FindObjectsOfType<LobbyClientGui>();
		GameObject old = found.Length == 0 ? null : found[0].gameObject;
		if (old != null)
			DestroyImmediate(old);
		
		NetworkManagerCustom.lobbyManager = GameObject.Find("LobbyManager");
		new SyncPlayersMessage().SendToServer();

		Destroy(GetComponent<NetworkLobby>());
	}
}
