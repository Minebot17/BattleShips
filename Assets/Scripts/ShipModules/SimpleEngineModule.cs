using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleEngineModule : AbstractModule, IEngineModule {

    [SerializeField] float trustPower;
    public float TrustPower { get { return trustPower * effectModule.freezeK; } }

    ShipController shipController;

    protected override void Start() {
        base.Start();
        shipController = transform.GetComponentInParent<ShipController>();
        if (shipController)
            shipController.engines.Add(this);
    }

    void OnDestroy() {
        if(shipController && shipController.engines.Contains(this)) 
            shipController.engines.Remove(this);
    }
}