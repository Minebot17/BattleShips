using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {
    
    private Vector2 lastGunVector;
    
    private Rigidbody2D rigidbody;
    private Transform forwardPointer;
    private NetworkIdentity identity;

    public GameObject enemyPointerPrefab;
    public float trustPower = 1f;

    public delegate void Engines(Vector2 direction, ForceMode2D mode);
    public Engines engines;

    public float rotationPower = 1f;
    private IInputHandler inputHandler = PlayerInputHandler.singleton;
    
    
    private void Start() {
        if (!isServer)
            MessageManager.RequestShipPartsServerMessage.SendToServer(new NetworkIdentityMessage(GetComponent<NetworkIdentity>()));
        identity = GetComponent<NetworkIdentity>();
        engines = AddMainForce;

        if (!hasAuthority) {
            GameObject enemyPointer = Instantiate(enemyPointerPrefab, GameObject.Find("Canvas").transform);
            enemyPointer.GetComponent<EnemyPointer>().Target = gameObject;
            return;
        }

        GameObject.Find("Main Camera").GetComponent<CameraFollower>().Target = gameObject.transform;
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
    }

    private void Update() {
        if (!hasAuthority)
            return;

        float rotation = inputHandler.GetShipRotation();
        float trust = inputHandler.GetShipTrust();
        Vector2 gunVector = inputHandler.GetGunVector(transform.position);

        if (lastGunVector != gunVector) {
            if (isServer)
                NetworkManagerCustom.singleton.playerGunVectors[identity] = gunVector;
            else
                CmdSendGunVector(gunVector);
            lastGunVector = gunVector;
        }

        if (rotation != 0)
            rigidbody.AddTorque(rotation * rotationPower, ForceMode2D.Force);
        
        if (trust != 0)
        {
            engines.Invoke(GetForward() * trust, ForceMode2D.Force);
        }
    }

    private Vector2 GetForward() {
        return (forwardPointer.position - forwardPointer.parent.position).ToVector2();
    }
    
    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendGunVector(Vector2 gunVector) {
        NetworkManagerCustom.singleton.playerGunVectors[identity] = gunVector;
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
