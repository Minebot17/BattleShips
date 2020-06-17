using UnityEngine;

class SimpleGyrodineEngine : AbstractModule, IGyrodineModule
{
    [SerializeField]
    private float rotationPower;
    public float RotationPower { get { return rotationPower * effectModule.freezeK; } }
    private ShipController shipController;

    protected override void Start() {
        base.Start();
        shipController = transform.GetComponentInParent<ShipController>();
        if (shipController)
            shipController.gyrodines.Add(this);
    }

    private void OnDestroy() {
        if (shipController && shipController.gyrodines.Contains(this)) 
            shipController.gyrodines.Remove(this);
    }

}

