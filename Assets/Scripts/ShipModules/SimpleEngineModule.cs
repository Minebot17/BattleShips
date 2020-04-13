using System;
using UnityEngine;
using UnityEngine.Networking;

public class SimpleEngineModule : MonoBehaviour, IEngineModule {

    [SerializeField]
    private float trustPower;
    public float TrustPower { get { return trustPower; } }

    private ShipController shipController;

    private void Start()
    {
        shipController = transform.GetComponentInParent<ShipController>();
        shipController.engines.Add(this);
    }

    private void OnDestroy()
    {
        if(shipController.engines.Contains(this)) shipController.engines.Remove(this);
    }
}