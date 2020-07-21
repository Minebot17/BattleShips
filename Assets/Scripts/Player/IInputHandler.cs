using System;
using UnityEngine;

public interface IInputHandler {
    float GetShipRotation();
    float GetShipTrust();
    bool GetGun();
    void Use(int index);
    void OnUse(Action<int> action);
}

