using UnityEngine;

public class ModuleHp : MonoBehaviour {
    
    [SerializeField] float health;

    public void Damage(DamageInfo bulletInfo) {
        if(transform.parent.parent.gameObject != bulletInfo.OwnerShip.gameObject)
        {
            health -= bulletInfo.Damage;

            if (health <= 0)
                GetComponent<IDeath>().OnDead(bulletInfo);
        }  
    }
}
