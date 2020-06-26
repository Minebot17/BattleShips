using UnityEngine;

public class MapTurret : MonoBehaviour {
    [SerializeField] private Transform forwardPointer;

    private AbstractGunModule gun;
    
    private void Start() {
        gun = GetComponent<AbstractGunModule>();
    }

    public void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (gun.CoolDown <= 0)
            gun.TryShoot((forwardPointer.position - transform.position).ToVector2().normalized);
    }
}
