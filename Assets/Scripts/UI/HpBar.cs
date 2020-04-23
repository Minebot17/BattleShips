public class HpBar : ProgressBar {
    
    private int eventId = -1;
    
    protected override void Start() {
        base.Start();
        CameraFollower.singleton.changeTargetEvent.SubcribeEvent(e => {
            ShipController controller;
            if (eventId != -1 && e.OldTarget && (controller = e.OldTarget.gameObject.GetComponent<ShipController>()))
                controller.moduleDeathEvent.UnSubcribeEvent(eventId);

            if (e.NewTarget && (controller = e.NewTarget.gameObject.GetComponent<ShipController>())) {
                if (controller.InitialModulesCount != 0)
                    UpdateProgressBar(controller.CurrentModulesCount, controller.InitialModulesCount);
                eventId = controller.moduleDeathEvent.SubcribeEvent(ev => {
                    if (controller.InitialModulesCount == 0)
                        return;
                    
                    UpdateProgressBar(controller.CurrentModulesCount, controller.InitialModulesCount);
                });
            }
        });
    }

    private void UpdateProgressBar(int currentHp, int maxHp) {
        float percent = NetworkManagerCustom.percentToDeath / 100f;
        Value = (currentHp - maxHp * percent) / (maxHp * (1f - percent));
    }
}
