using UnityEngine;
using UnityEngine.Networking;

public class KillShipClientMessage : GameMessage {

    public KillShipClientMessage() { }

    public KillShipClientMessage(NetworkIdentity killer, NetworkIdentity prey) {
        Writer.Write(killer);
        Writer.Write(prey);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity killer = reader.ReadNetworkIdentity();
        NetworkIdentity prey = reader.ReadNetworkIdentity();

        if (prey.hasAuthority) {
            PlayerInputHandler.singleton.ToggleInput(false);
            CameraFollower.singleton.gameObject.AddComponent<PlayerObserver>();
        }
        else {
            EnemyPointer[] pointers = GameObject.FindObjectsOfType<EnemyPointer>();
            foreach (EnemyPointer pointer in pointers)
                if (pointer.Target == prey.gameObject)
                    MonoBehaviour.Destroy(pointer.gameObject);
        }

        prey.GetComponent<IDeath>().OnDead(null);
    }
}