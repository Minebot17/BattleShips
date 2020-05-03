using UnityEngine.Networking;

public class BoolStateValue : StateValue<bool> {
    public BoolStateValue(PlayerState parent, string name, bool defaultValue, bool sync = false, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }

    protected override bool ReadValue(NetworkReader reader) {
        return reader.ReadBoolean();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}
