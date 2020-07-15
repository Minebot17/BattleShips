using System;
using UnityEngine;
using UnityEngine.UI;

public class ShieldToggle : MonoBehaviour {

    private Toggle toggle;
    private CommonState cState;
    
    private void Start() {
        cState = Players.GetClient().GetState<CommonState>();
        toggle = GetComponent<Toggle>();
        toggle.SetIsOnWithoutNotify(cState.WithShield.Value);
    }

    public void OnToggle(bool isOn) {
        cState.WithShield.Value = isOn;
    }
}
