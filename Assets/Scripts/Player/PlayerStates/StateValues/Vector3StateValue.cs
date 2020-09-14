using UnityEngine;
using UnityEngine.Networking;

public class Vector3StateValue : StateValue<Vector3> {
    
    public Vector3StateValue(PlayerState parent, string name, Vector3 defaultValue, SyncType sync = SyncType.NOT_SYNC, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }
    
    protected override Vector3 ReadValue(NetworkReader reader) {
        return reader.ReadVector3();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}