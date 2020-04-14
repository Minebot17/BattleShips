using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {

    public GameObject enemyPointerPrefab;
    public float trustPower = 1f;
    public float rotationPower = 1f;

    public List<IEngineModule> engines = new List<IEngineModule>();
    public List<IGyrodineModule> gyrodines = new List<IGyrodineModule>();   
    
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

        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");

        if (!hasAuthority) {
            GameObject enemyPointer = Instantiate(enemyPointerPrefab, GameObject.Find("Canvas").transform);
            enemyPointer.GetComponent<EnemyPointer>().Target = gameObject;
            return;
        }

        CameraFollower.singleton.Target = gameObject.transform;
    }

    private void FixedUpdate() {
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
            rigidbody.AddTorque(rotation * (rotationPower + gyrodines.Sum(e => e.RotationPower)), ForceMode2D.Force);
        
        if (trust != 0)
            rigidbody.AddForce(GetForward() * (trustPower + engines.Sum(e => e.TrustPower)), ForceMode2D.Force);        
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
}
