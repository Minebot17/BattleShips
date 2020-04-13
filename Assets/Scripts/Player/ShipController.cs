using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {

    public delegate void Engines(Vector2 direction, ForceMode2D mode);
    public GameObject enemyPointerPrefab;
    public float trustPower = 1f;
    public Engines engines;
    public float rotationPower = 1f;
    
    private bool lastGunButton;
    private Rigidbody2D rigidbody;
    private Transform forwardPointer;
    private NetworkIdentity identity;
    private IInputHandler inputHandler;
    
    
    private void Start() {
        inputHandler = PlayerInputHandler.singleton;
        if (!isServer)
            MessageManager.RequestShipPartsServerMessage.SendToServer(new NetworkIdentityMessage(GetComponent<NetworkIdentity>()));
        identity = GetComponent<NetworkIdentity>();
        engines = AddMainForce;

        if (!hasAuthority) {
            GameObject enemyPointer = Instantiate(enemyPointerPrefab, GameObject.Find("Canvas").transform);
            enemyPointer.GetComponent<EnemyPointer>().Target = gameObject;
            return;
        }

        CameraFollower.singleton.Target = gameObject.transform;
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
    }

    private void Update() {
        if (!hasAuthority)
            return;

        float rotation = inputHandler.GetShipRotation();
        float trust = inputHandler.GetShipTrust();
        bool gunButton = inputHandler.GetGun();

        if (lastGunButton != gunButton) {
            if (isServer)
                NetworkManagerCustom.singleton.playersGunButton[identity] = gunButton;
            else
                CmdSendGunVector(gunButton);
            lastGunButton = gunButton;
        }

        if (rotation != 0)
            rigidbody.AddTorque(rotation * rotationPower, ForceMode2D.Force);
        
        if (trust != 0)
            engines.Invoke(GetForward() * trust, ForceMode2D.Force);
        
    }

    public Vector2 GetForward() {
        return (forwardPointer.position - forwardPointer.parent.position).ToVector2();
    }
    
    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendGunVector(bool gunButton) {
        NetworkManagerCustom.singleton.playersGunButton[identity] = gunButton;
    }
    
    public override int GetNetworkChannel() {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval() {
        return 0.02f;
    }

    private void AddMainForce(Vector2 direction, ForceMode2D mode)
    {
        rigidbody.AddForce(direction * trustPower, mode);
    }
}
