public class LobbyState : PlayerState {

    public BoolStateValue Ready;
    
    public LobbyState(Player parent, bool isTest) : base(parent, isTest) {
        Ready = new BoolStateValue(this, "Ready", false, SyncType.ALL_SYNC, true);
    }
}
