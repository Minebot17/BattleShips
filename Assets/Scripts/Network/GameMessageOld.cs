using UnityEngine.Networking;

public class GameMessageOld {

	private short index;
	private NetworkMessageDelegate method;

	public GameMessageOld(NetworkMessageDelegate method) {
		index = MessageManagerOld.LastIndex;
		this.method = method;
		MessageManagerOld.LastIndex++;
		MessageManagerOld.ToRegister.Add(this);
	}

	public void Register() {
		NetworkManager.singleton.client.RegisterHandler(index, method);
		NetworkServer.RegisterHandler(index, method);
	}

	public void SendToServer(MessageBase message) {
		NetworkManager.singleton.client.Send(index, message);
	}

	public void SendToClient(NetworkConnection connection, MessageBase message) {
		connection.Send(index, message);
		NetworkWriter writer = new NetworkWriter();
		
	}

	public void SendToAllClients(MessageBase message) {
		NetworkServer.SendToAll(index, message);
	}

	public short GetIndex() {
		return index;
	}
}
