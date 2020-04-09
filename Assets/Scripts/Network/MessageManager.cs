using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

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
	
	/*public static readonly GameMessage RequestShipClientMessage = new GameMessage(msg => {
		ResponseShipServerMessage.SendToServer(new StringMessage(Utils.GetShipJson(NetworkLobbyClientGUI.selectedShip)));
	});
	
	public static readonly GameMessage ResponseShipServerMessage = new GameMessage(msg => {
		GameObject.Find("LobbyManager").GetComponent<NetworkLobbyServerGUI>()
				.AddClientShip(msg.conn, msg.ReadMessage<StringMessage>().value);
	});*/
	
	public static readonly GameMessage RequestShipPartsServerMessage = new GameMessage(msg => {
		NetworkIdentity id = msg.ReadMessage<NetworkIdentityMessage>().Value;
		ResponseShipPartsClientMessage.SendToClient(msg.conn, new MessagesMessage(new MessageBase[] {
			new NetworkIdentityMessage(id),
			new StringMessage(NetworkManagerCustom.singleton.playerShips[id.clientAuthorityOwner])
		}));
	});
	
	public static readonly GameMessage ResponseShipPartsClientMessage = new GameMessage(msg => {
		MessagesList messages = msg.ReadMessage<MessagesMessage>().Value;
		GameObject shipObject = ((NetworkIdentityMessage) messages[0]).Value.gameObject;
		Utils.DeserializeShipPartsFromJson(shipObject, ((StringMessage)messages[1]).value);
	});
	
	public static readonly GameMessage DestroyModuleClientMessage = new GameMessage(msg => {
		MessagesMessage messages = msg.ReadMessage<MessagesMessage>();
		NetworkIdentity identity = ((NetworkIdentityMessage)messages.Value[0]).Value;
		string cellName = ((StringMessage) messages.Value[1]).value;
		MonoBehaviour.Destroy(identity.transform.Find(cellName).transform.GetChild(0).gameObject);
	});

	public static readonly GameMessage GameOverClientMessage = new GameMessage(msg => {
		bool winner = bool.Parse(msg.ReadMessage<StringMessage>().value);
		GameObject.Find(winner ? "WinLabel" : "LooseLabel").GetComponent<Text>().enabled = true;
		GameObject.Find("RestartButton").GetComponent<Image>().enabled = true;
		GameObject.Find("RestartButton").GetComponent<Button>().enabled = true;
		GameObject.Find("RestartButton").transform.GetChild(0).GetComponent<Text>().enabled = true;
		Time.timeScale = 0;
	});
	
	public static readonly GameMessage ResetGameServerMessage = new GameMessage(msg => {
		NetworkManager.singleton.ServerChangeScene("Game");
		Time.timeScale = 1f;
		RecoveryTimeScaleClientMessage.SendToAllClients(new EmptyMessage());
	});
	
	public static readonly GameMessage RecoveryTimeScaleClientMessage = new GameMessage(msg => {
		Time.timeScale = 1f;
	});
	
	public static readonly GameMessage RequestShipEditorServerMessage = new GameMessage(msg => {
		ResponseShipEditorClientMessage.SendToClient(msg.conn, new StringMessage(NetworkManagerCustom.singleton.playerShips[msg.conn]));
	});
	
	public static readonly GameMessage ResponseShipEditorClientMessage = new GameMessage(msg => {
		ShipEditor.singleton.OpenShip(msg.ReadMessage<StringMessage>().value);
	});
	
	public static readonly GameMessage SendShipServerMessage = new GameMessage(msg => {
		NetworkManagerCustom.singleton.playerShips[msg.conn] = msg.ReadMessage<StringMessage>().value;
		if (--NetworkManagerCustom.singleton.lastConnections == 0) 
			NetworkManagerCustom.singleton.ServerChangeScene("Game");
	});
	
	[Serializable]
	public class MessagesList : List<MessageBase> { }

	[Serializable]
	public class StringList : List<string> { }

	[Serializable]
	public class MultyStringList : List<List<string>> { }
	
	[Serializable]
	public class Vector3List : List<Vector3> { }

	#endregion
}
