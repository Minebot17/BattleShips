using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {
    
    public EventHandler<ModuleDeathEvent> moduleDeathEvent = new EventHandler<ModuleDeathEvent>();
    
    public List<IEngineModule> engines = new List<IEngineModule>();
    public List<IGyrodineModule> gyrodines = new List<IGyrodineModule>();

    private bool lastGunButton;
    private Rigidbody2D rigidbody;
    private Transform forwardPointer;
    private NetworkIdentity identity;
    private IInputHandler inputHandler;

    private int initialModulesCount;
    private int currentModulesCount;
    private GameObject aiCoreModule;

    public int InitialModulesCount => initialModulesCount;
    public int CurrentModulesCount => currentModulesCount;

    private void Start() {
        inputHandler = PlayerInputHandler.singleton;
        identity = GetComponent<NetworkIdentity>();
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
        new ShipPartsMessage(identity).SendToServer();
    }

    private void FixedUpdate() {
        if (!hasAuthority)
            return;

        float rotation = inputHandler.GetShipRotation();
        float trust = inputHandler.GetShipTrust();
        bool gunButton = inputHandler.GetGun();

        if (lastGunButton != gunButton) {
            if (isServer)
                Players.GetPlayer(identity.clientAuthorityOwner).GetState<CommonState>().IsShoot.Value = gunButton;
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
        aiCoreModule = transform.Find("ShipCell 0 0").GetChild(0).gameObject;
    }

    public void OnModuleDeath(Transform cell) {
        currentModulesCount--;
        if (cell.childCount != 0)
            Destroy(cell.GetChild(0).gameObject);

        moduleDeathEvent.CallListners(new ModuleDeathEvent(gameObject));
    }

    public GameObject GetAiCoreModule() {
        return aiCoreModule;
    }

    [Command(channel = Channels.DefaultUnreliable)]
    private void CmdSendGunButton(bool gunButton) {
        Players.GetPlayer(identity.clientAuthorityOwner).GetState<CommonState>().IsShoot.Value = gunButton;
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
