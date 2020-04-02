using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {
    private Rigidbody2D rigidbody;
    private Transform forwardPointer;

    public float trustPower = 1f;
    public float rotationPower = 1f;
    
    private void Start() {
        GameObject.Find("Main Camera").GetComponent<CameraFollower>().Target = gameObject;
        if (!isLocalPlayer)
            return;

        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
        ShipInputManager.singleton.playerShip = gameObject;
    }

    private void FixedUpdate() {
        if (!isLocalPlayer)
            return;

        float rotation = ShipInputManager.singleton.GetShipRotation();
        float trust = ShipInputManager.singleton.GetShipTrust();
        Vector2 gunVector = ShipInputManager.singleton.GetGunVector();
        
        if (rotation != 0)
            rigidbody.AddTorque(rotation * rotationPower, ForceMode2D.Force);
        
        if (trust != 0)
            rigidbody.AddForce(forwardPointer.localPosition.ToVector2() * (trust * trustPower), ForceMode2D.Force);
        
        // TODO gun
    }
}
