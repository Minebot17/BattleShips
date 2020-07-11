public class GlobalState : PlayerState {
    
    public StringStateValue CurrentMapName;
    public IntStateValue RoundTime;
    public IntStateValue RoundsCount;

    public GlobalState(Player parent, bool isTest) : base(parent, isTest) {
        CurrentMapName = new StringStateValue(this, "CurrentMapName", "WallsAndSpikesMap", true, true);
        RoundTime = new IntStateValue(this, "RoundTime", 120, true, true);
        RoundsCount = new IntStateValue(this, "RoundsCount", 4, true, true);
    }
}