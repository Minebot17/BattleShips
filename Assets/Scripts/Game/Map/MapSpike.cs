using System;
using UnityEngine;
using UnityEngine.Networking;

public class MapSpike : MonoBehaviour {

    [SerializeField] private float damage;
    [SerializeField] private int perTicks;
    private long ticksTimer;

    private void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        ticksTimer++;
    }

    private void OnTriggerStay2D(Collider2D other) {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        if (ticksTimer % perTicks == 0) {
            if (other.gameObject.transform.parent == null || !other.gameObject.transform.parent.gameObject.tag.Equals("Player"))
                return;

            ModuleHp hp = other.gameObject.transform.GetChild(0).gameObject.GetComponent<ModuleHp>();
            if (hp) {
                hp.Damage(new DamageInfo(damage, null));
            }
        }
    }
}
