using UnityEngine.Networking;

public abstract class GeneralStateValue {
    public abstract void Write(NetworkWriter writer);
    public abstract void Read(NetworkReader reader);
    public abstract void OnRemoveState();
}
