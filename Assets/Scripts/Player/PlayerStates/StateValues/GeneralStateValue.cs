using UnityEngine.Networking;

public abstract class GeneralStateValue {
    public abstract string GetName();
    public abstract void Write(NetworkWriter writer);
    public abstract void Read(NetworkReader reader, NetworkConnection modificationBuffer);
    public abstract void OnRemoveState();
    public abstract void Reset();
}
