using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class DeathStandart : MonoBehaviour, IDeath
{
    public void OnDead() {
        Destroy(gameObject);
        MessageManager.DestroyModuleClientMessage.SendToAllClients(new MessagesMessage(new MessageBase[] {
            new NetworkIdentityMessage(transform.parent.parent.GetComponent<NetworkIdentity>()),
            new StringMessage(transform.parent.gameObject.name) 
        }));
    }
}
