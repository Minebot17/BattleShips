using System;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

[Serializable]
public class MessagesMessage : MessageBase {
    public MessageManager.MessagesList Value = new MessageManager.MessagesList();
    
    public MessagesMessage() { }
    
    public MessagesMessage(MessageBase[] value) {
        Value = new MessageManager.MessagesList();
        Value.AddRange(value);
    }
		
    public override void Deserialize(NetworkReader reader) {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++) {
            string typeStr = reader.ReadString();
            Type type = typeStr.Equals("UnityEngine.Networking.NetworkSystem.StringMessage")
                        ? typeof(StringMessage)
                        : typeStr.Equals("UnityEngine.Networking.NetworkSystem.IntegerMessage") 
                        ? typeof(IntegerMessage) 
                        : Type.GetType(typeStr);
            MessageBase message = (MessageBase) type.GetConstructor(new Type[0]).Invoke(new object[0]);
            message.Deserialize(reader);
            Value.Add(message);
        }
    }

    public override void Serialize(NetworkWriter writer) {
        writer.Write(Value.Count);
        foreach (MessageBase value in Value) {
            writer.Write(value.GetType().ToString());
            value.Serialize(writer);
        }
    }
}