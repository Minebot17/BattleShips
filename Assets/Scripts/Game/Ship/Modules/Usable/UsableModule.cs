using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public abstract class UsableModule : AbstractModule {

    [SerializeField] private float coolDown;
    protected ShipServerController serverController;
    protected NetworkIdentity shipIdentity;

    public float CoolDown => coolDown;

    protected void Awake() {
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        if (gameObject.name.EndsWith("(Clone)"))
            gameObject.name = gameObject.name.Substring(0, gameObject.name.Length - 7);
            
        serverController = transform.parent.parent.GetComponent<ShipServerController>();
        shipIdentity = transform.parent.parent.GetComponent<NetworkIdentity>();
    }

    private void OnEnable() {
        if (!NetworkManagerCustom.singleton.IsServer || !SceneManager.GetActiveScene().name.Equals("Game"))
            return;
        
        int siblingIndex = transform.parent.GetSiblingIndex();
        UsableModuleInfo moduleIndexes = serverController.usableModules.Get(gameObject.name);
        if (moduleIndexes == null) {
            moduleIndexes = new UsableModuleInfo(new List<int> { siblingIndex }, false);
            serverController.usableModules[gameObject.name] = moduleIndexes;

            new EditUseButtonClientMessage(gameObject.name, false).SendToClient(shipIdentity.clientAuthorityOwner);
        }
        else
            moduleIndexes.sameModulesIndex.Add(siblingIndex);
    }

    private void OnDisable() {
        if (!NetworkManagerCustom.singleton.IsServer || !SceneManager.GetActiveScene().name.Equals("Game"))
            return;
        
        int siblingIndex = transform.parent.GetSiblingIndex();
        UsableModuleInfo moduleIndexes = serverController.usableModules.Get(gameObject.name);
        if (moduleIndexes.sameModulesIndex.Count == 1) {
            serverController.usableModules[gameObject.name] = null;
            
            new EditUseButtonClientMessage(gameObject.name, true).SendToClient(shipIdentity.clientAuthorityOwner);
        }
        else
            moduleIndexes.sameModulesIndex.Remove(siblingIndex);
    }

    protected int GetInstalledCount() {
        return serverController.usableModules[gameObject.name].sameModulesIndex.Count;
    }

    public abstract void Use();
}