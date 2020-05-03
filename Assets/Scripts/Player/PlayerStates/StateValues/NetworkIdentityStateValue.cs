using UnityEngine.Networking;

public class NetworkIdentityStateValue : StateValue<NetworkIdentity> {
    public NetworkIdentityStateValue(PlayerState parent, string name, NetworkIdentity defaultValue, bool sync = false, bool modification = false) 
    : base(parent, name, defaultValue, parent.IsTest(), sync, modification) { }
    
    protected override NetworkIdentity ReadValue(NetworkReader reader) {
        return reader.ReadNetworkIdentity();
    }

    public override void Write(NetworkWriter writer) {
        writer.Write(Value);
    }
}
