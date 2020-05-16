using UnityEngine;
using UnityEngine.Networking;

public class CreateExplosionClientMessage : GameMessage {

    public CreateExplosionClientMessage() { }

    public CreateExplosionClientMessage(int type, float lifeTime, Vector2 position) {
        Writer.Write(type);
        Writer.Write(lifeTime);
        Writer.Write(position);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        ExplosionManager.singleton.SpawnExplosionPrefab(reader.ReadInt32(), reader.ReadSingle(), reader.ReadVector2());
    }
}