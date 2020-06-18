using System.Collections.Generic;
using UnityEngine.Networking;

public class DamageInfo {
    public NetworkIdentity OwnerShip { get; private set; }
    public float Damage { get; set; }

    public List<IEffectFabric> effects = new List<IEffectFabric>();

    public DamageInfo(float damage, NetworkIdentity ownerShip) {
        OwnerShip = ownerShip;
        Damage = damage;
    }
}
