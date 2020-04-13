using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking.NetworkSystem;
using UnityEngine.UI;

public class PlayerInputHandler : MonoBehaviour, IInputHandler {
    public static PlayerInputHandler singleton;
    public Camera cam;
    public bool touch = true;
    public Joystick leftJoystick;
    public HoldButton gunButton;
    public GameObject screenButton;

    public void Awake() {
        singleton = this;
    }

    public void Start() {
        leftJoystick.gameObject.SetActive(touch);
        gunButton.gameObject.SetActive(touch);
    }

    public void ToggleInput(bool active) {
        if (touch) {
            leftJoystick.gameObject.SetActive(active);
            gunButton.gameObject.SetActive(active);
        }
    }

    public float GetShipRotation() {
        if (touch)
            return -leftJoystick.Horizontal;
        
        return Input.GetKey(KeyCode.A) ? 1 : Input.GetKey(KeyCode.D) ? -1 : 0;
    }

    public float GetShipTrust() {
        if (touch)
            return leftJoystick.Vertical;
        
        return Input.GetKey(KeyCode.S) ? -1 : Input.GetKey(KeyCode.W) ? 1 : 0;
    }

    public bool GetGun() {
        return touch ? gunButton.buttonPressed : Input.GetMouseButton(0);
    }
    
    public void OnRestartClick() {
		MessageManager.ResetGameServerMessage.SendToServer(new EmptyMessage());
    }
}
