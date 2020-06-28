using System;
using UnityEngine;

public class ModuleHp : MonoBehaviour, IOnModuleDeathServer {
    
    public EventHandler<DamageEvent> damageEvent = new EventHandler<DamageEvent>();
    [SerializeField] private float health;
    [SerializeField] private float currentHealth;

    public float MaxHealth => health;
    public float CurrentHealth => currentHealth;

    public void Awake() {
        currentHealth = health;
    }

    public void Damage(DamageInfo damageInfo) {
        if(!damageInfo.OwnerShip || transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject) {
            DamageEvent result = damageEvent.CallListners(new DamageEvent(this, damageInfo));
            if (result.IsCancel)
                return;
            
            currentHealth -= result.DamageInfo.Damage;

            if (currentHealth <= 0)
                GetComponent<IDeath>().OnDead(result.DamageInfo);
        }  
    }
    
    public void OnModuleDeath() {
        damageEvent.UnSubcribeAll();
    }

    public class DamageEvent : EventBase {
        
        public DamageInfo DamageInfo;
        
        public DamageEvent(object sender, DamageInfo damageInfo) : base(sender, true) {
            DamageInfo = damageInfo;
        }
    }
}
