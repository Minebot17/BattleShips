using System;
using UnityEngine;
using UnityEngine.Networking;

public class HpBar : ProgressBar {
    private int eventId = -1;
    private EventHandler<FloatStateValue.OnChangeValueEvent> eventHandler;
    
    protected override void Start() {
        base.Start();
        CameraFollower.singleton.changeTargetEvent.SubcribeEvent(e => {
            ShipController controller;
            if (eventId != -1 && e.OldTarget && (controller = e.OldTarget.gameObject.GetComponent<ShipController>()))
                controller.moduleDeathEvent.UnSubcribeEvent(eventId);

            if (e.NewTarget && (controller = e.NewTarget.gameObject.GetComponent<ShipController>())) {
                ModuleHp aiCoreHp = controller.GetAiCoreModule()?.GetComponent<ModuleHp>();
                if (aiCoreHp == null)
                    return;
                
                CommonState cState = Players.GetPlayer(e.NewTarget.gameObject.GetComponent<NetworkIdentity>()).GetState<CommonState>();
                Value = cState.CurrentHealth.Value / aiCoreHp.MaxHealth;
                eventHandler = cState.CurrentHealth.onChangeValueEvent;
                eventId = eventHandler.SubcribeEvent(ev => {
                    Value = Math.Max(ev.NewValue / aiCoreHp.MaxHealth, 0);
                });
            }
        });
    }

    private void OnDestroy() {
        if (eventId != -1)
            eventHandler.UnSubcribeEvent(eventId);
    }
}
