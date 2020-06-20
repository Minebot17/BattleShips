using UnityEngine;

public class ModuleHp : MonoBehaviour {
    
    [SerializeField] float health;

    public void Damage(DamageInfo damageInfo) {
        if(transform.parent.parent.gameObject != damageInfo.OwnerShip.gameObject)
        {
            health -= damageInfo.Damage;

            if (health <= 0)
                GetComponent<IDeath>().OnDead(damageInfo);
        }  
    }
}
