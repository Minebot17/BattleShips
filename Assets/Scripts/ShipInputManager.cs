using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipInputManager : MonoBehaviour {
    public static ShipInputManager singleton;
    public GameObject playerShip; // надо задавать программно. Нужно, чтобы вычислять вектор направления стрельбы на пк
    public Camera cam;
    public bool touch = true;
    public Joystick leftJoystick;
    public Joystick rightJoystick;
    
    public void Awake() {
        singleton = this;
    }

    public void Start() {
        leftJoystick.enabled = touch;
        rightJoystick.enabled = touch;
    }

    public float GetShipRotation() {
        if (touch)
            return leftJoystick.Horizontal;
        
        return Input.GetKey(KeyCode.A) ? -1 : Input.GetKey(KeyCode.D) ? 1 : 0;
    }

    public float GetShipTrust() {
        if (touch)
            return leftJoystick.Vertical;
        
        return Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;
    }

    public Vector2 GetGunVector() {
        if (touch)
            return rightJoystick.Direction;
        
        return Input.GetMouseButton(0) 
               ? (cam.ScreenToWorldPoint(Input.mousePosition) - playerShip.transform.position).ToVector2() 
               : new Vector2(0, 0);
    }
}
