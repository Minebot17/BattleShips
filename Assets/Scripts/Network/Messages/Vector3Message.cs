using System;
using UnityEngine;
using UnityEngine.Networking;

public class Vector3Message : MessageBase {
    public Vector3 Value;

    public Vector3Message() { }

    public Vector3Message(Vector3 value) {
        Value = value;
    }
	
    public override void Deserialize(NetworkReader reader) {
        Value = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    public override void Serialize(NetworkWriter writer) {
        writer.Write(Value.x);
        writer.Write(Value.y);
        writer.Write(Value.z);
    }
}
