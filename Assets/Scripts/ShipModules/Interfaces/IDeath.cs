using UnityEngine.Networking;

public interface IDeath {
    void OnDead(DamageInfo damageInfo);
    bool IsDead();
}
