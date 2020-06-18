using UnityEngine;

public class ModuleHp : MonoBehaviour {
    
    [SerializeField] float health;

    public void Damage(DamageInfo bulletInfo) {
        health -= bulletInfo.Damage;
        
        if (health <= 0)
            GetComponent<IDeath>().OnDead(bulletInfo);
    }
}
