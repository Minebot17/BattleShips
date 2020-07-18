public class InventoryState : PlayerState {

    public IntStateValue[] modulesCount;

    public InventoryState(Player parent, bool isTest) : base(parent, isTest) {
        modulesCount = new IntStateValue[ShipEditor.modules.Length];
        for (int i = 0; i < modulesCount.Length; i++) {
            EditorModule module = ShipEditor.modules[i];
            modulesCount[i] = new IntStateValue(
                this, 
                "modulesCount_" + i,
                module.availableInitially ? (ShipEditor.modules[i].endlessModule ? -1 : ShipEditor.modules[i].startAmount) : 0,
                SyncType.OWNER_SYNC
            );
        }
    }
}
