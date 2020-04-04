using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleHp : MonoBehaviour {
    
    [SerializeField]
    private int health;

    public void Damage(int damage) {
        health -= damage;
        
        if (health <= 0)
            GetComponent<IDeath>().OnDead();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.gameObject.GetComponent<IAmmo>() != null)
            other.gameObject.GetComponent<IAmmo>().OnCollide(this);
    }
}
