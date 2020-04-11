
using UnityEngine.Networking;

public class PlayerDamageSource : DamageSource {
    private NetworkIdentity ownerShip;

    public NetworkIdentity OwnerShip => ownerShip;

    public PlayerDamageSource(int amount, NetworkIdentity ownerShip) : base(amount) {
        this.ownerShip = ownerShip;
    }
}
