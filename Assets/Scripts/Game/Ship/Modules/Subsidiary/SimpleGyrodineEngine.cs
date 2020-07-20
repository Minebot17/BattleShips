using UnityEngine;

internal class SimpleGyrodineEngine : AbstractModule, IGyrodineModule
{
    [SerializeField] private float rotationPower;
    public float RotationPower { get { return rotationPower * effectModule.freezeK; } }
    private ShipController shipController;

    protected void OnEnable() {
        shipController = transform.GetComponentInParent<ShipController>();
        if (shipController)
            shipController.gyrodines.Add(this);
    }

    private void OnDisable() {
        if (shipController && shipController.gyrodines.Contains(this)) 
            shipController.gyrodines.Remove(this);
    }

}

