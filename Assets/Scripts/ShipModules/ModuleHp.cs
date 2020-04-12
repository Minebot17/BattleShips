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
        if (other.gameObject.GetComponent<IAmmo>() != null)
            other.gameObject.GetComponent<IAmmo>().OnCollide(this);
    }
}
