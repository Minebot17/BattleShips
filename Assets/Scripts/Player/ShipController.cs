using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {
    
    public EventHandler<ModuleDeathEvent> moduleDeathEvent = new EventHandler<ModuleDeathEvent>();
    
    public List<IEngineModule> engines = new List<IEngineModule>();
    public List<IGyrodineModule> gyrodines = new List<IGyrodineModule>();

    bool lastGunButton;
    Rigidbody2D rigidbody;
    Transform forwardPointer;
    NetworkIdentity identity;
    IInputHandler inputHandler;

    int initialModulesCount;
    int currentModulesCount;

    public int InitialModulesCount => initialModulesCount;
    public int CurrentModulesCount => currentModulesCount;

    void Start() {
        inputHandler = PlayerInputHandler.singleton;
        identity = GetComponent<NetworkIdentity>();
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");

        if (hasAuthority)
            CameraFollower.singleton.Target = gameObject.transform;
        
        new ShipPartsMessage(identity).SendToServer();
    }

    void FixedUpdate() {
        if (!hasAuthority)
            return;

        float rotation = inputHandler.GetShipRotation();
        float trust = inputHandler.GetShipTrust();
        bool gunButton = inputHandler.GetGun();

        if (lastGunButton != gunButton) {
            if (isServer)
                Players.GetPlayer(identity.clientAuthorityOwner).GetState<GameState>().IsShoot.Value = gunButton;
            else
                CmdSendGunButton(gunButton);
            lastGunButton = gunButton;
        }

        if (Mathf.Abs(rotation) > 0.25f)
            rigidbody.AddTorque(rotation * (gyrodines.Sum(e => e.RotationPower)), ForceMode2D.Force);
        
        if (Mathf.Abs(trust) > 0.25f)
            rigidbody.AddForce(GetForward() * (engines.Sum(e => e.TrustPower)), ForceMode2D.Force);        
    }

    public Vector2 GetForward() {
        return (forwardPointer.position - forwardPointer.parent.position).ToVector2();
    }

    public void OnInitializePartsOnClient() {
        initialModulesCount = GetComponentsInChildren<ModuleHp>().Length;
        currentModulesCount = initialModulesCount;
    }

    public void OnModuleDeath(Transform cell) {
        currentModulesCount--;
        if (cell.childCount != 0)
            MonoBehaviour.Destroy(cell.GetChild(0).gameObject);

        moduleDeathEvent.CallListners(new ModuleDeathEvent(gameObject));
    }

    [Command(channel = Channels.DefaultUnreliable)]
    void CmdSendGunButton(bool gunButton) {
        Players.GetPlayer(identity.clientAuthorityOwner).GetState<GameState>().IsShoot.Value = gunButton;
    }
    
    public override int GetNetworkChannel() {
        return Channels.DefaultUnreliable;
    }

    public override float GetNetworkSendInterval() {
        return 0.02f;
    }

    public class ModuleDeathEvent : EventBase {
        public ModuleDeathEvent(GameObject sender) : base(sender, false) { }
    }
}
