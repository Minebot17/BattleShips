using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public Button[] usableButtons = new Button[3];
    private Action<int> onUse;
    private readonly KeyCode[] useKeys = { KeyCode.Q, KeyCode.E, KeyCode.R };

    public void Awake() {
        singleton = this;
    }

    public void Start() {
        leftJoystick.gameObject.SetActive(touch);
        gunButton.gameObject.SetActive(touch);
    }

    public void Update() {
        if (touch)
            return;
        
        for (int i = 0; i < useKeys.Length; i++)
            if (Input.GetKeyDown(useKeys[i]))
                Use(i);
    }

    public void ToggleInput(bool active) {
        if (touch) {
            leftJoystick.gameObject.SetActive(active);
            gunButton.gameObject.SetActive(active);
            usableButtons[0].gameObject.SetActive(active);
            usableButtons[1].gameObject.SetActive(active);
            usableButtons[2].gameObject.SetActive(active);
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

    public void Use(int index) {
        onUse.Invoke(index);
    }

    public void OnUse(Action<int> action) {
        onUse = action;
    }

    public void EditUseButton(string moduleName, bool remove) {
        if (!touch)
            return;

        if (remove) {
            GameObject moduleButton = usableButtons.First(b => b.gameObject.name.EndsWith(moduleName)).gameObject;
            moduleButton.SetActive(false);
        }
        else {
            GameObject firstDisabled = usableButtons.First(b => !b.gameObject.activeSelf).gameObject;
            firstDisabled.SetActive(true);
            firstDisabled.name = firstDisabled.name.Split('_')[0] + "_" + moduleName;
            firstDisabled.transform.GetChild(0).GetComponent<Image>().sprite = 
                ShipEditor.modules.First(em => em.prefab.name.Equals(moduleName)).prefab.GetComponent<SpriteRenderer>().sprite;
        }

        SortUsableButtons();
    }

    private void SortUsableButtons() {
        Button[] isEnabledButtons = usableButtons.Where(b => b.gameObject.activeSelf).ToArray();
        for (int i = 0; i < isEnabledButtons.Length; i++)
            isEnabledButtons[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(-150 * i, 300);
    }
}
