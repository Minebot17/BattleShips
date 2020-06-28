using UnityEngine.Networking;

public class FloatStateValue : StateValue<float> {
    public FloatStateValue(PlayerState parent, string name, float defaultValue, bool sync = false, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }
    
    protected override float ReadValue(NetworkReader reader) {
        return reader.ReadSingle();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}
