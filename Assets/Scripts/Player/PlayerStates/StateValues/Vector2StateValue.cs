using UnityEngine;
using UnityEngine.Networking;

public class Vector2StateValue : StateValue<Vector2> {
    
    public Vector2StateValue(PlayerState parent, string name, Vector3 defaultValue, SyncType sync = SyncType.NOT_SYNC, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }
    
    protected override Vector2 ReadValue(NetworkReader reader) {
        return reader.ReadVector2();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}