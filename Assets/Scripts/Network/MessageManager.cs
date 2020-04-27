using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class MessageManager {

	public static short LastIndex = 99;
	public static readonly List<GameMessage> ToRegister = new List<GameMessage>();
	public static bool Inited;

	public static void Initialize() {
		if (Inited)
			return;
		
		foreach (GameMessage message in ToRegister)
			message.Register();

		Inited = true;
	}

	// ClientMessage - server to server, ServerMessage - client to server
	#region messages
	
	public static readonly GameMessage RequestLobbyModeServerMessage = new GameMessage(msg => {
		ResponseLobbyModeClientMessage.SendToClient(msg.conn,
			new StringListMessage(NetworkManagerCustom.singleton.StartArguments));
	});
	
	public static readonly GameMessage ResponseLobbyModeClientMessage = new GameMessage(msg => {
		NetworkManagerCustom.singleton.StartArguments = msg.ReadMessage<StringListMessage>().Value;
		string mode = NetworkManagerCustom.singleton.StartArguments.Find(s => s.StartsWith("gamemode:")).Split(':')[1];
		GameObject lobbyManager = GameObject.Find("LobbyManager");
		
		switch (mode) {
			case "ffa":
				lobbyManager.AddComponent<LobbyClientGUI>();
				break;
			case "commands":
				lobbyManager.AddComponent<LobbyClientTeamGUI>();
				break;
		}
	});

	public static readonly GameMessage SetReadyLobbyServerMessage = new GameMessage(msg => {
		bool ready = bool.Parse(msg.ReadMessage<StringMessage>().value);
		GameObject.Find("LobbyManager").GetComponent<LobbyServerGui>().SetReady(msg.conn, ready);
	});
	
	public static readonly GameMessage RequestShipPartsServerMessage = new GameMessage(msg => {
		NetworkIdentity id = msg.ReadMessage<NetworkIdentityMessage>().Value;
		ResponseShipPartsClientMessage.SendToClient(msg.conn, new MessagesMessage(new MessageBase[] {
			new NetworkIdentityMessage(id),
			new StringMessage(NetworkManagerCustom.singleton.playerData[id.clientAuthorityOwner].ShipJson)
		}));
	});
	
	public static readonly GameMessage ResponseShipPartsClientMessage = new GameMessage(msg => {
		MessagesList messages = msg.ReadMessage<MessagesMessage>().Value;
		NetworkIdentity shipObject = ((NetworkIdentityMessage) messages[0]).Value;
		string json = ((StringMessage) messages[1]).value;
		Utils.DeserializeShipPartsFromJson(shipObject.gameObject, json);

		ShipController controller = shipObject.gameObject.GetComponent<ShipController>();
		if (controller) 
			controller.OnInitializePartsOnClient();
	});
	
	public static readonly GameMessage DestroyModuleClientMessage = new GameMessage(msg => {
		MessagesMessage messages = msg.ReadMessage<MessagesMessage>();
		NetworkIdentity identity = ((NetworkIdentityMessage)messages.Value[0]).Value;
		string cellName = ((StringMessage) messages.Value[1]).value;
		Transform cellTransform = identity.transform.Find(cellName);
		identity.gameObject.GetComponent<ShipController>().OnModuleDeath(cellTransform);
	});

	public static readonly GameMessage RequestShipEditorServerMessage = new GameMessage(msg => {
		ResponseShipEditorClientMessage.SendToClient(msg.conn, new StringMessage(NetworkManagerCustom.singleton.playerData[msg.conn].ShipJson));
	});
	
	public static readonly GameMessage ResponseShipEditorClientMessage = new GameMessage(msg => {
		ShipEditor.singleton.OpenShip(msg.ReadMessage<StringMessage>().value);
	});
	
	public static readonly GameMessage SendShipServerMessage = new GameMessage(msg => {
		NetworkManagerCustom.singleton.playerData[msg.conn].ShipJson = msg.ReadMessage<StringMessage>().value;
		if (--NetworkManagerCustom.singleton.lastConnections == 0) 
			NetworkManagerCustom.singleton.ServerChangeScene("Game");
	});
	
	public static readonly GameMessage KillShipClientMessage = new GameMessage(msg => {
		MessagesMessage messages = msg.ReadMessage<MessagesMessage>();
		NetworkIdentity killer = ((NetworkIdentityMessage) messages.Value[0]).Value;
		NetworkIdentity prey = ((NetworkIdentityMessage) messages.Value[1]).Value;

		if (prey.hasAuthority) {
			PlayerInputHandler.singleton.ToggleInput(false);
			CameraFollower.singleton.gameObject.AddComponent<PlayerObserver>();
		}
		else {
			EnemyPointer[] pointers = GameObject.FindObjectsOfType<EnemyPointer>();
			foreach (EnemyPointer pointer in pointers)
				if (pointer.Target == prey.gameObject)
					MonoBehaviour.Destroy(pointer.gameObject);
		}

		prey.GetComponent<IDeath>().OnDead(null);
	});

	public static readonly GameMessage RequestScoreboardInfoServerMessage = new GameMessage(msg => {
		ResponseScoreboardInfoClientMessage.SendToClient(msg.conn, new MessagesMessage(new MessageBase[] {
			new StringListMessage(NetworkManagerCustom.singleton.playerData.Values.Select(d => d.ShipJson).ToList()),
			new IntegerListMessage(NetworkManagerCustom.singleton.playerData.Values.Select(d => d.Score).ToList()), 
			new IntegerListMessage(NetworkManagerCustom.singleton.playerData.Values.Select(d => d.Kills).ToList()), 
			new IntegerMessage(NetworkManagerCustom.singleton.scoreForWin) 
		}));
	});
	
	public static readonly GameMessage ResponseScoreboardInfoClientMessage = new GameMessage(msg => {
		MessagesMessage messages = msg.ReadMessage<MessagesMessage>();
		Scoreboard.singleton.Init(
			((StringListMessage)messages.Value[0]).Value,
			((IntegerListMessage)messages.Value[1]).Value,
			((IntegerListMessage)messages.Value[2]).Value,
			((IntegerMessage)messages.Value[3]).value
		);
	});
	
	public static readonly GameMessage RequestTimerInEditorServerMessage = new GameMessage(msg => {
		ResponseTimerInEditorClientMessage.SendToClient(msg.conn, new IntegerMessage(ShipEditor.singleton.timeBeforeClosing));
	});
	
	public static readonly GameMessage ResponseTimerInEditorClientMessage = new GameMessage(msg => {
		ShipEditor.singleton.SetTimer(msg.ReadMessage<IntegerMessage>().value - 1); // -1 нужно чтобы у клиентов таймер чуть был меньше из-за задержки пакетов
	});
	
	public static readonly GameMessage SuicideServerMessage = new GameMessage(msg => {
		NetworkManagerCustom.singleton.PlayerKill(null, NetworkManagerCustom.singleton.playerData[msg.conn].ShipIdentity);
	});
	
	public static readonly GameMessage RequestEnemyPointerColorServerMessage = new GameMessage(msg => {
		NetworkIdentity target = msg.ReadMessage<NetworkIdentityMessage>().Value;
		ResponseEnemyPointerColorServerMessage.SendToClient(msg.conn, new MessagesMessage(new MessageBase[] {
			new NetworkIdentityMessage(target),
			new IntegerMessage(NetworkManagerCustom.singleton.gameMode.GetEnemyPointerColor(msg.conn, target)) 
		}));
	});
	
	public static readonly GameMessage ResponseEnemyPointerColorServerMessage = new GameMessage(msg => {
		MessagesList messages = msg.ReadMessage<MessagesMessage>().Value;
		NetworkIdentity target = ((NetworkIdentityMessage) messages[0]).Value;
		int color = ((IntegerMessage) messages[1]).value;
		
		GameObject enemyPointer = MonoBehaviour.Instantiate(
			NetworkManagerCustom.singleton.enemyPointerPrefab, GameObject.Find("Canvas").transform);
		enemyPointer.GetComponent<EnemyPointer>().Target = target.gameObject;
		enemyPointer.GetComponentInChildren<Image>().color = color.ToColor();
	});

	public static readonly GameMessage SendPlayerIdClientMessage = new GameMessage(msg => {
		NetworkManagerCustom.singleton.clientIndex = msg.ReadMessage<IntegerMessage>().value;
	});
	
	#region "Team GUI synchronization"
	
	public static readonly GameMessage SendNickServerMessage = new GameMessage(msg => {
		string nick = msg.ReadMessage<StringMessage>().value;
		NetworkManagerCustom.singleton.playerData[msg.conn].Nick = nick.Equals("ip") ? msg.conn.address : nick;
	});
	
	#endregion
	
	[Serializable]
	public class MessagesList : List<MessageBase> { }

	[Serializable]
	public class StringList : List<string> { }

	[Serializable]
	public class MultyStringList : List<List<string>> { }
	
	[Serializable]
	public class Vector3List : List<Vector3> { }
	
	[Serializable]
	public class IntegerList : List<int> {  }

	#endregion
}
