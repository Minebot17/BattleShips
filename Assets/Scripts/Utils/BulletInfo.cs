using System.Collections.Generic;
using UnityEngine.Networking;

public class BulletInfo {
    public NetworkIdentity OwnerShip { get; private set; }
    public float Damage { get; private set; }

    public List<IModuleEffect> effects = new List<IModuleEffect>();

    public BulletInfo(float damage, NetworkIdentity ownerShip) {
        OwnerShip = ownerShip;
        Damage = damage;
    }
}
