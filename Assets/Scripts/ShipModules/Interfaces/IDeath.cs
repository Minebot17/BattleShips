using UnityEngine.Networking;

public interface IDeath {
    void OnDead(DamageSource source);
    bool IsDead();
}
