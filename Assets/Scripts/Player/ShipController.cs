using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {
    
    public EventHandler<ModuleDeathEvent> moduleDeathEvent = new EventHandler<ModuleDeathEvent>();
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
    
    private int initialModulesCount;
    private int currentModulesCount;

    public int InitialModulesCount => initialModulesCount;
    public int CurrentModulesCount => currentModulesCount;

    private void Start() {
        inputHandler = PlayerInputHandler.singleton;
        identity = GetComponent<NetworkIdentity>();
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
        
        if (hasAuthority) 
            CameraFollower.singleton.Target = gameObject.transform;
        else
            new EnemyPointerColorMessage(
            GameObject.FindGameObjectsWithTag("Player").First(g => g.GetComponent<NetworkIdentity>().hasAuthority).GetComponent<NetworkIdentity>(), 
            identity).SendToServer();

        if (!isServer) {
            new ShipPartsMessage(GetComponent<NetworkIdentity>()).SendToServer();
        }
        else {
            initialModulesCount = GetComponentsInChildren<ModuleHp>().Length;
            currentModulesCount = initialModulesCount;
        }
    }

    private void FixedUpdate() {
        if (!hasAuthority)
            return;

        float rotation = inputHandler.GetShipRotation();
        float trust = inputHandler.GetShipTrust();
        bool gunButton = inputHandler.GetGun();

        if (lastGunButton != gunButton) {
            if (isServer)
                NetworkManagerCustom.singleton.playerData[identity.clientAuthorityOwner].IsShoot = gunButton;
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
    private void CmdSendGunVector(bool gunButton) {
        NetworkManagerCustom.singleton.playerData[identity.clientAuthorityOwner].IsShoot = gunButton;
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
