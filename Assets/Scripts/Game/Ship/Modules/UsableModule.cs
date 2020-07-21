using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public abstract class UsableModule : AbstractModule {

    [SerializeField] private float coolDown;
    private ShipServerController serverController;
    private NetworkIdentity identity;

    public float CoolDown => coolDown;

    protected override void Start() {
        base.Start();
        if (!NetworkManagerCustom.singleton.IsServer)
            return;
        
        serverController = transform.parent.parent.GetComponent<ShipServerController>();
        identity = transform.parent.parent.GetComponent<NetworkIdentity>();
    }

    private void OnEnable() {
        if (!NetworkManagerCustom.singleton.IsServer || !SceneManager.GetActiveScene().name.Equals("Game"))
            return;
        
        int siblingIndex = transform.parent.GetSiblingIndex();
        UsableModuleInfo moduleIndexes = serverController.usableModules[gameObject.name];
        if (moduleIndexes == null) {
            moduleIndexes = new UsableModuleInfo(new List<int> { siblingIndex }, false);
            serverController.usableModules[gameObject.name] = moduleIndexes;

            new EditUseButtonClientMessage(gameObject.name, false).SendToClient(identity.clientAuthorityOwner);
        }
        else
            moduleIndexes.sameModulesIndex.Add(siblingIndex);
    }

    private void OnDisable() {
        if (!NetworkManagerCustom.singleton.IsServer || !SceneManager.GetActiveScene().name.Equals("Game"))
            return;
        
        int siblingIndex = transform.parent.GetSiblingIndex();
        UsableModuleInfo moduleIndexes = serverController.usableModules[gameObject.name];
        if (moduleIndexes.sameModulesIndex.Count == 1) {
            serverController.usableModules[gameObject.name] = null;
            
            new EditUseButtonClientMessage(gameObject.name, true).SendToClient(identity.clientAuthorityOwner);
        }
        else
            moduleIndexes.sameModulesIndex.Remove(siblingIndex);
    }

    public abstract void Use();
}