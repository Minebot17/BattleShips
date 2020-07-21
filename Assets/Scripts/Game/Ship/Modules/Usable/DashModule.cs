using UnityEngine;

public class DashModule : UsableModule {
    
    public override void Use() {
        Vector2 forward = shipIdentity.GetComponent<ShipController>().GetForward();
        Rigidbody2D rigidbody = shipIdentity.GetComponent<Rigidbody2D>();
        rigidbody.AddForce(forward * 7f * Mathf.Sqrt(GetInstalledCount()), ForceMode2D.Impulse);
        rigidbody.MarkServerChange();
    }
}
