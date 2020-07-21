using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class RepairModule : AbstractModule {

    [SerializeField] private float repairPerSeconds;
    private ShipServerController serverController;
    private NetworkIdentity identity;
    private CommonState cState;

    protected override void Start() {
        base.Start();
        if (!NetworkManagerCustom.singleton.IsServer)
            return;

        serverController = transform.parent.parent.GetComponent<ShipServerController>();
        identity = transform.parent.parent.GetComponent<NetworkIdentity>();
        cState = Players.GetPlayer(identity).GetState<CommonState>();
        StartCoroutine(RepairCoroutine());
    }

    private IEnumerator RepairCoroutine() {
        while (true) {
            if (cState.IsInvisible.Value)
                yield return new WaitForSeconds(1f);
            
            if (serverController && serverController.killedModules.Count != 0) {
                int repairIndex = Utils.rnd.Next(serverController.killedModules.Count);
                GameObject toRepair = serverController.killedModules[repairIndex];
                serverController.killedModules.RemoveAt(repairIndex);
                toRepair.transform.parent.GetComponent<BoxCollider2D>().enabled = true;
                toRepair.SetActive(true);
                ModuleHp hp = toRepair.GetComponent<ModuleHp>();
                hp.CurrentHealth = hp.MaxHealth;
                toRepair.GetComponent<IDeath>().Repair();
                new RepairModuleClientMessage(identity, toRepair.transform.parent.GetSiblingIndex()).SendToAllClientExceptHost();
            }

            yield return new WaitForSeconds(repairPerSeconds * ((2f - effectModule.freezeK)/0.75f));
        }
    }
}
