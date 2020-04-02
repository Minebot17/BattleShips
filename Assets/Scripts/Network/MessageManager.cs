using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class MessageManager {

	public static short LastIndex = 99;
	public static readonly List<GameMessage> ToRegister = new List<GameMessage>();

	public static void Initialize() {
		foreach (GameMessage message in ToRegister)
			message.Register();
	}

	// ClientMessage - server to server, ServerMessage - client to server
	#region messages
	
	public static readonly GameMessage RequestLobbyModeServerMessage = new GameMessage(msg => {
		ResponseLobbyModeClientMessage.SendToClient(msg.conn,
			new StringListMessage(NetworkManagerCustom.singleton.StartArguments));
	});
	
	public static readonly GameMessage ResponseLobbyModeClientMessage = new GameMessage(msg => {
		NetworkManagerCustom.singleton.StartArguments = msg.ReadMessage<StringListMessage>().Value;
		GameObject.Find("LobbyManager").AddComponent<NetworkLobbyClientGUI>();
	});
	
	public static readonly GameMessage SetReadyLobbyServerMessage = new GameMessage(msg => {
		bool ready = bool.Parse(msg.ReadMessage<StringMessage>().value);
		GameObject.Find("LobbyManager").GetComponent<NetworkLobbyServerGUI>().SetReady(msg.conn, ready);
	});
	
	public static readonly GameMessage RequestShipClientMessage = new GameMessage(msg => {
		ResponseShipServerMessage.SendToServer(new StringMessage(Utils.GetShipJson(NetworkLobbyClientGUI.selectedShip)));
	});
	
	public static readonly GameMessage ResponseShipServerMessage = new GameMessage(msg => {
		GameObject.Find("LobbyManager").GetComponent<NetworkLobbyServerGUI>()
				.AddClientShip(msg.conn, msg.ReadMessage<StringMessage>().value);
	});

	[Serializable]
	public class StringList : List<string> { }

	[Serializable]
	public class MultyStringList : List<List<string>> { }
	
	[Serializable]
	public class Vector3List : List<Vector3> { }

	#endregion
}
