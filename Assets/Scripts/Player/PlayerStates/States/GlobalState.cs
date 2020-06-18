public class GlobalState : PlayerState {
    
    public StringStateValue CurrentMapName;

    public GlobalState(Player parent, bool isTest) : base(parent, isTest) {
        CurrentMapName = new StringStateValue(this, "CurrentMapName", Utils.mapNotSelected, true, true);
    }
}