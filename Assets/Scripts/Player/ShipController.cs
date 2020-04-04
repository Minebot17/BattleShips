using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {
    
    private Vector2 lastGunVector;
    
    private Rigidbody2D rigidbody;
    private Transform forwardPointer;
    private NetworkIdentity identity;

    public float trustPower = 1f;
    public float rotationPower = 1f;
    
    
    private void Start() {
        MessageManager.RequestShipPartsServerMessage.SendToServer(new NetworkIdentityMessage(GetComponent<NetworkIdentity>()));
        identity = GetComponent<NetworkIdentity>();
        
        if (!hasAuthority)
            return;

        GameObject.Find("Main Camera").GetComponent<CameraFollower>().Target = gameObject;
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
        ShipInputManager.singleton.playerShip = gameObject;
    }

    private void FixedUpdate() {
        if (!hasAuthority)
            return;

        float rotation = ShipInputManager.singleton.GetShipRotation();
        float trust = ShipInputManager.singleton.GetShipTrust();
        Vector2 gunVector = ShipInputManager.singleton.GetGunVector();

        if (lastGunVector != gunVector) {
            if (isServer)
                NetworkManagerCustom.singleton.playerGunVectors[identity] = gunVector;
            else
                CmdSendGunVector(gunVector);
            lastGunVector = gunVector;
        }

        if (rotation != 0)
            rigidbody.AddTorque(rotation * rotationPower, ForceMode2D.Force);
        
        if (trust != 0)
            rigidbody.AddForce(GetForward() * (trust * trustPower), ForceMode2D.Force);
    }

    private Vector2 GetForward() {
        return (forwardPointer.position - forwardPointer.parent.position).ToVector2();
    }
    
    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendGunVector(Vector2 gunVector) {
        NetworkManagerCustom.singleton.playerGunVectors[identity] = gunVector;
    }
    
    public override int GetNetworkChannel() {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval() {
        return 0.02f;
    }
}
