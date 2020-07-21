using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

public class UseModuleServerMessage : GameMessage {

    public UseModuleServerMessage() { }

    public UseModuleServerMessage(string moduleName, bool fromName = true) {
        Writer.Write(fromName);
        Writer.Write(moduleName);
    }

    public UseModuleServerMessage(int index, bool fromName = false) {
        Writer.Write(fromName);
        Writer.Write(index);
    }
    
    public override void OnServer(NetworkReader reader, NetworkConnection conn) {
        NetworkIdentity shipIdentity = Players.GetPlayer(conn).GetState<CommonState>().ShipIdentity.Value;
        ShipServerController serverController = shipIdentity.GetComponent<ShipServerController>();
        try {
            UsableModuleInfo usableModuleInfo = reader.ReadBoolean()
                                                ? serverController.usableModules[reader.ReadString()]
                                                : serverController.usableModules.Values.ToList()[reader.ReadInt32()];

            if (usableModuleInfo != null
                && usableModuleInfo.sameModulesIndex != null 
                && usableModuleInfo.sameModulesIndex.Count != 0 
                && !usableModuleInfo.isCoolDown) {
                UsableModule usableModule = shipIdentity
                                        .transform.GetChild(usableModuleInfo.sameModulesIndex[0]).GetChild(0)
                                        .GetComponent<UsableModule>();
                usableModule.Use();
                usableModuleInfo.isCoolDown = true;
                serverController.StartCoroutine(CoolDownTimer(usableModule.CoolDown, usableModuleInfo));
            }
        }
        catch (IndexOutOfRangeException e) {
            Debug.Log(e);
        }
        catch (NullReferenceException e) {
            Debug.Log(e);
        }
    }

    private IEnumerator CoolDownTimer(float seconds, UsableModuleInfo usableModuleInfo) {
        yield return new WaitForSeconds(seconds);
        usableModuleInfo.isCoolDown = false;
    }

    public override void OnClient(NetworkReader reader) {
        
    }
}