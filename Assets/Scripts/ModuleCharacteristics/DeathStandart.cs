using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class DeathStandart : NetworkBehaviour, IDeath
{
    public void OnDead() {
        NetworkServer.Destroy(gameObject);
    }
}
