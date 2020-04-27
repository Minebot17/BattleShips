using System.Collections.Generic;
using UnityEngine.Networking;

[System.Serializable]
public class StringListMessage : MessageBase {
	public MessageManagerOld.StringList Value = new MessageManagerOld.StringList();

	public StringListMessage() { }

	public StringListMessage(List<string> value) {
		Value = new MessageManagerOld.StringList();
		Value.AddRange(value);
	}
		
	public override void Deserialize(NetworkReader reader) {
		int count = reader.ReadInt32();
		for (int i = 0; i < count; i++)
			Value.Add(reader.ReadString());
	}

	public override void Serialize(NetworkWriter writer) {
		writer.Write(Value.Count);
		foreach (string value in Value) {
			writer.Write(value);
		}
	}
}