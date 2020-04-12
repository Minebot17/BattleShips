using UnityEngine;

public interface IInputHandler {
    float GetShipRotation();
    float GetShipTrust();
    Vector2 GetGunVector(Vector3 vector);
}

