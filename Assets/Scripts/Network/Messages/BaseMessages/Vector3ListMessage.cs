using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Vector3ListMessage : MessageBase {
    public Utils.Vector3List Value = new Utils.Vector3List();

    public Vector3ListMessage() { }

    public Vector3ListMessage(List<Vector3> value) {
        Value = new Utils.Vector3List();
        Value.AddRange(value);
    }
		
    public override void Deserialize(NetworkReader reader) {
        int count = reader.ReadInt32();
        for (int i = 0; i < count; i++)
            Value.Add(reader.ReadVector3());
    }

    public override void Serialize(NetworkWriter writer) {
        writer.Write(Value.Count);
        foreach (Vector3 value in Value)
            writer.Write(value);
    }
}
