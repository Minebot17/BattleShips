using UnityEngine;
using UnityEngine.Networking;

public interface IAmmo {
    void Init(GameObject owner, Vector2 shootVector);
    void OnCollide(ModuleHp hp);
}
