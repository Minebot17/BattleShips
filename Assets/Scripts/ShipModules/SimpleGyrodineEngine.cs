using UnityEngine;

internal class SimpleGyrodineEngine : AbstractModule, IGyrodineModule
{
    [SerializeField] float rotationPower;
    public float RotationPower { get { return rotationPower * effectModule.freezeK; } }
    ShipController shipController;

    protected override void Start() {
        base.Start();
        shipController = transform.GetComponentInParent<ShipController>();
        if (shipController)
            shipController.gyrodines.Add(this);
    }

    void OnDestroy() {
        if (shipController && shipController.gyrodines.Contains(this)) 
            shipController.gyrodines.Remove(this);
    }

}

