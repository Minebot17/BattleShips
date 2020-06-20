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
        ParticleSystem particleSystem = reader.ReadNetworkIdentity().transform.GetChild(reader.ReadInt32()).GetComponentInChildren<ParticleSystem>();
        if (reader.ReadBoolean())
            particleSystem.Play();
        else
            particleSystem.Stop();
    }
}