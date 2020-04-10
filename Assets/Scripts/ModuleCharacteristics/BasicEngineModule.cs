using System;
using UnityEngine;
using UnityEngine.Networking;

public class BasicEngineModule : MonoBehaviour, IEngineModule {

    [SerializeField] private float trustPower;
    private Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = transform.GetComponentInParent<Rigidbody2D>();
        transform.GetComponentInParent<ShipController>().engines += AddForce;
    }

    private void OnDestroy()
    {
        transform.GetComponentInParent<ShipController>().engines -= AddForce;
    }

    public void AddForce(Vector2 direction, ForceMode2D mode)
    {
        rigidbody.AddForceAtPosition(direction * trustPower,transform.position, ForceMode2D.Force);
    }
}