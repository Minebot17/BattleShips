using UnityEngine.Networking;

// children must have empty constructor
public abstract class GameMessage {
    public NetworkWriter Writer = new NetworkWriter();

    protected GameMessage() {
        if (MessageManager.MessageIndexes == null)
            return;
            
        short messageId = MessageManager.MessageIndexes[GetType()];
        Writer.StartMessage(messageId);
    }

    public abstract void OnClient(NetworkReader reader);
    public abstract void OnServer(NetworkReader reader, NetworkConnection conn);

    public int GetChannel() {
        return Channels.DefaultReliable;
    }
}
