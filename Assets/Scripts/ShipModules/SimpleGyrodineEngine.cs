using UnityEngine;

class SimpleGyrodineEngine : MonoBehaviour, IGyrodineModule
{
    [SerializeField]
    private float rotationPower;
    public float RotationPower { get { return rotationPower; } }
    private ShipController shipController;

    private void Start()
    {
        shipController = transform.GetComponentInParent<ShipController>();
        shipController.gyrodines.Add(this);
    }

    private void OnDestroy()
    {
        if (shipController.gyrodines.Contains(this)) shipController.gyrodines.Remove(this);
    }

}

