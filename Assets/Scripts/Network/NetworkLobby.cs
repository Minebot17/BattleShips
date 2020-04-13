using UnityEngine;
using UnityEngine.Networking.NetworkSystem;

public class NetworkLobby : MonoBehaviour {
	
	private void Awake() {
		MessageManager.Initialize();
		DontDestroyOnLoad(gameObject);
	}

	private void Start() {
		NetworkLobbyClientGUI[] finded = FindObjectsOfType<NetworkLobbyClientGUI>();
		GameObject old = finded.Length == 0 ? null : finded[0].gameObject;
		if (old != null)
			Destroy(old);
		
		if (NetworkManagerCustom.singleton.IsServer)
			GameObject.Find("LobbyManager").AddComponent<NetworkLobbyServerGUI>();
		else
			MessageManager.RequestLobbyModeServerMessage.SendToServer(new EmptyMessage());
		Destroy(GetComponent<NetworkLobby>());
	}
}
