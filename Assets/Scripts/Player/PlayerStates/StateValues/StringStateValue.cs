using UnityEngine.Networking;

public class StringStateValue : StateValue<string> {
    public StringStateValue(PlayerState parent, string name, string defaultValue, SyncType sync = SyncType.NOT_SYNC, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }
    
    protected override string ReadValue(NetworkReader reader) {
        return reader.ReadString();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}
