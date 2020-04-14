using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleEngineModule : MonoBehaviour, IEngineModule {

    [SerializeField]
    private float trustPower;
    public float TrustPower { get { return trustPower; } }

    private ShipController shipController;

    private void Start() {
        shipController = transform.GetComponentInParent<ShipController>();
        
        if (shipController)
            shipController.engines.Add(this);
    }

    private void OnDestroy() {
        if(shipController && shipController.engines.Contains(this)) 
            shipController.engines.Remove(this);
    }
}