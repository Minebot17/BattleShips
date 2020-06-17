﻿using UnityEngine;

public class ModuleHp : MonoBehaviour {
    
    [SerializeField] float health;

    public void Damage(BulletInfo bulletInfo) {
        health -= bulletInfo.Damage;
        
        if (health <= 0)
            GetComponent<IDeath>().OnDead(bulletInfo);
    }
}
