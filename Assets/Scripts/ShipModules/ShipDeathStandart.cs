using System;
using UnityEngine;

public class ShipDeathStandart : MonoBehaviour, IDeath {

    [SerializeField] private GameObject effectPrefab;
    [SerializeField] private int destroyTime;
    private GameObject effect;
    
    public void OnDead(DamageSource source) {
        effect = Instantiate(effectPrefab);
        effect.transform.localPosition = new Vector3(transform.position.x, transform.position.y, 0.1f);
        Destroy(effect, destroyTime);
        gameObject.SetActive(false);
        Destroy(gameObject, destroyTime + 0.2f);
    }
}