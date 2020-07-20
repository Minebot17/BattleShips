using System;
using UnityEngine;

public class ShieldModule : MonoBehaviour {

    [SerializeField] private int plusShieldHp;
    
    private void Start() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
            
        Transform shield = transform.parent.parent.Find("ShieldRenderer");
        if (shield)
            shield.GetComponentInChildren<ModuleHp>().MaxHealth += plusShieldHp;
    }
}