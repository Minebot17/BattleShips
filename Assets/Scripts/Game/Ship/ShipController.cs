using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class ShipController : NetworkBehaviour {

    [Tooltip("Affects the rotation of the ship during movement. The bigger the harder it is to turn")]
    public float atmosphericDensity = 1f;
    public Shader shieldShader;
    public List<IEngineModule> engines = new List<IEngineModule>();
    public List<IGyrodineModule> gyrodines = new List<IGyrodineModule>();

    private bool lastGunButton;
    private Rigidbody2D rigidbody;
    private Transform forwardPointer;
    private NetworkIdentity identity;
    private IInputHandler inputHandler;
    private GameObject aiCoreModule;

    private void Start() {
        inputHandler = PlayerInputHandler.singleton;
        identity = GetComponent<NetworkIdentity>();
        rigidbody = GetComponent<Rigidbody2D>();
        forwardPointer = transform.Find("ForwardPointer");
        
        if (!gameObject.transform.Find("ShilCell 0 0"))
            new ShipPartsMessage(identity).SendToServer();

        Action<int> onUseModule = index => {
            if (PlayerInputHandler.singleton.touch) {
                string moduleName = PlayerInputHandler
                                .singleton.usableButtons
                                .First(b => b.gameObject.name.StartsWith("UsableModule" + index)).gameObject.name
                                .Split('_')[1];
                new UseModuleServerMessage(moduleName).SendToServer();
            }
            else
                new UseModuleServerMessage(index).SendToServer();
        };
        inputHandler.OnUse(onUseModule);
    }

    private void FixedUpdate() {
        if (!hasAuthority)
            return;

        float rotation = inputHandler.GetShipRotation();
        float trust = inputHandler.GetShipTrust();
        bool gunButton = inputHandler.GetGun();
        
        // If speed is faster, turning is harder
        rotation *= 1f/(rigidbody.velocity.magnitude * atmosphericDensity + 1f);

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
        aiCoreModule = transform.Find("ShipCell 0 0").GetChild(0).gameObject;
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
}
