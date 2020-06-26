using UnityEngine;

public class HpBar : ProgressBar {
    private int eventId = -1;
    
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
                
                Value = aiCoreHp.CurrentHealth / aiCoreHp.MaxHealth;
                eventId = aiCoreHp.damageEvent.SubcribeEvent(ev => {
                    Value = (aiCoreHp.CurrentHealth - ev.DamageInfo.Damage) / aiCoreHp.MaxHealth;
                    Value = Value < 0 ? 0 : Value;
                });
            }
        });
    }

    private void UpdateProgressBar(int currentHp, int maxHp) {
        
    }
}
