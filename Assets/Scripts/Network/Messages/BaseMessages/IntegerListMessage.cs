using System.Collections.Generic;
using UnityEngine.Networking;

public class IntegerListMessage : MessageBase {
    public Utils.IntegerList Value = new Utils.IntegerList();

    public IntegerListMessage() { }

    public IntegerListMessage(List<int> value) {
        Value = new Utils.IntegerList();
        Value.AddRange(value);
    }
		
    public override void Deserialize(NetworkReader reader) {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            Value.Add(reader.ReadInt32());
    }

    public override void Serialize(NetworkWriter writer) {
        writer.Write(Value.Count);
        foreach (int value in Value) {
            writer.Write(value);
        }
    }
}
