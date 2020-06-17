using UnityEngine.Networking;

public interface IDeath {
    void OnDead(BulletInfo bulletInfo);
    bool IsDead();
}
