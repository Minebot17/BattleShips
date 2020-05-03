using UnityEngine;
using UnityEngine.Networking;

public interface IAmmo {
    void Initialize(GameObject owner, Vector2 shootVector);
    void OnCollide(ModuleHp hp);
    DamageSource GetDamageSource();
}
