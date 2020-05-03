using UnityEngine;
using UnityEngine.Networking;

public class CommonGunModule : MonoBehaviour, IGunModule {

    [SerializeField]
    private int ticksCD;

    [SerializeField]
    private float ammoSpeed;

    [SerializeField]
    private GameObject ammoPrefab;

    private int timerCD;
    
    public void Shoot(Vector2 vec) {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        if (timerCD <= 0) {
            GameObject ammo = Instantiate(ammoPrefab);
            ammo.transform.position = transform.position.ToVector2() + vec/2f;
            ammo.transform.position += new Vector3(0, 0, -0.2f);
            ammo.GetComponent<IAmmo>().Initialize(transform.parent.parent.gameObject, (vec * ammoSpeed));

            NetworkServer.Spawn(ammo);
            timerCD = ticksCD;
        }
    }

    public void FixedUpdate() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        if (timerCD > 0)
            timerCD--;
    }
}