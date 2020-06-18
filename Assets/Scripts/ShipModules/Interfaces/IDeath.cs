using UnityEngine.Networking;

public interface IDeath {
    void OnDead(DamageInfo bulletInfo);
    bool IsDead();
}
