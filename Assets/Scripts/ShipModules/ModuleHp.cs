using UnityEngine;

public class ModuleHp : MonoBehaviour {
    
    [SerializeField]
    private int health;

    public void Damage(DamageSource source) {
        health -= source.Amount;
        
        if (health <= 0)
            GetComponent<IDeath>().OnDead(source);
    }

    private void OnTriggerEnter2D(Collider2D other) {
       
        AbstractAmmo ammo = null;
        if ((ammo = other.gameObject.GetComponent<AbstractAmmo>()) != null && 
                NetworkManagerCustom.singleton.gameMode.CanDamageModule(this, ammo.GetDamageSource()))
            ammo.OnCollide(this);
    }
}
