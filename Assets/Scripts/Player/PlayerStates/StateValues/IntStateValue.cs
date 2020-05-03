using UnityEngine.Networking;

public class IntStateValue : StateValue<int> {
    public IntStateValue(PlayerState parent, string name, int defaultValue, bool sync = false, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }
    
    protected override int ReadValue(NetworkReader reader) {
        return reader.ReadInt32();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}
