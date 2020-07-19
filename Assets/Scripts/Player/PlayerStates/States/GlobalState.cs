public class GlobalState : PlayerState {
    
    public StringStateValue CurrentMapName;
    public IntStateValue RoundTime;
    public IntStateValue RoundsCount;
    public IntStateValue BuildPointsPerRound;
    public BoolStateValue WithLootItems;

    public GlobalState(Player parent, bool isTest) : base(parent, isTest) {
        CurrentMapName = new StringStateValue(this, "CurrentMapName", "WallsAndSpikesMap", SyncType.ALL_SYNC, true);
        RoundTime = new IntStateValue(this, "RoundTime", 120, SyncType.ALL_SYNC, true);
        RoundsCount = new IntStateValue(this, "RoundsCount", 4, SyncType.ALL_SYNC, true);
        BuildPointsPerRound = new IntStateValue(this, "BuildPointsPerRound", 3, SyncType.ALL_SYNC, true);
        WithLootItems = new BoolStateValue(this, "WithLootItems", false, SyncType.ALL_SYNC, true);
    }
}