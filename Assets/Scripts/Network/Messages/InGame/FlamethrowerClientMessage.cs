using System;
using UnityEngine;
using UnityEngine.Networking;

public class FlamethrowerClientMessage : GameMessage {

    public FlamethrowerClientMessage() { }

    public FlamethrowerClientMessage(NetworkIdentity identity, int childIndex, bool play) {
        Writer.Write(identity);
        Writer.Write(childIndex);
        Writer.Write(play);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        
    }
    
    public override void OnClient(NetworkReader reader) {
        NetworkIdentity id = reader.ReadNetworkIdentity();
        int childIndex = reader.ReadInt32();
        ParticleSystem particleSystem = childIndex == -1 ? id.gameObject.GetComponent<ParticleSystem>() : id.transform.GetChild(childIndex).GetComponentInChildren<ParticleSystem>();
        if (reader.ReadBoolean())
            particleSystem.Play();
        else
            particleSystem.Stop();
    }
}